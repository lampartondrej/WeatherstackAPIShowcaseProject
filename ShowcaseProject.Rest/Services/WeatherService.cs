using Microsoft.Extensions.Caching.Memory;
using ShowcaseProject.RestApi.CustomHelpers;
using ShowcaseProject.Services.Interfaces;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Shared;
using ShowcaseProject.Shared.Model.Wrapper;
using System.Net;
using System.Text.Json;

namespace ShowcaseProject.Services
{
    /// <summary>
    /// Service implementation for retrieving weather information from the Weatherstack API.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly ILogger<WeatherService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IWeatherstackRequestBuilder _requestBuilder;
        private readonly string WeatherApiKey;
        private readonly string WeatherstackApiKeyEnvVar;
        private readonly string WeatherApiBaseUrl;
        private readonly string CurrentWeatherEndpoint;
        private readonly string ForecastWeatherEndpoint;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public WeatherService(ILogger<WeatherService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache,
            IWeatherstackRequestBuilder requestBuilder)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _requestBuilder = requestBuilder;
            WeatherstackApiKeyEnvVar = configuration.GetValue<string>("APIOptions:WeatherstackApiKeyEnvVar") ?? throw new InvalidOperationException("Weather API key environment variable name is not set in configuration.");
            WeatherApiKey = Environment.GetEnvironmentVariable($"{WeatherstackApiKeyEnvVar}") ?? throw new InvalidOperationException($"Weather API key is not set in environment variable '{WeatherstackApiKeyEnvVar}'.");
            WeatherApiBaseUrl = configuration.GetValue<string>("APIOptions:WeatherstackApiUrl") ?? throw new InvalidOperationException("Weather API base URL is not set in configuration.");
            CurrentWeatherEndpoint = configuration.GetValue<string>("APIOptions:CurrentWeatherEndpoint") ?? throw new InvalidOperationException("Current weather endpoint is not set in configuration.");
            ForecastWeatherEndpoint = configuration.GetValue<string>("APIOptions:ForecastWeatherEndpoint") ?? throw new InvalidOperationException("Forecast weather endpoint is not set in configuration.");
        }

        #region public methods
        public Task<ServiceWrapper<CurrentWeatherResponse>> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest)
        {
            return GetWeatherAsync<CurrentWeatherResponse>(
                cacheKey: BuildCurrentWeatherCacheKey(currentWeatherRequest),
                endpoint: CurrentWeatherEndpoint,
                buildQuery: () => _requestBuilder.BuildQueryForCurrentWeather(currentWeatherRequest),
                location: currentWeatherRequest.Location,
                dataDescription: "current weather data");
        }

        public Task<ServiceWrapper<ForecastWeatherResponse>> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest)
        {
            return GetWeatherAsync<ForecastWeatherResponse>(
                cacheKey: BuildForecastWeatherCacheKey(forecastWeatherRequest),
                endpoint: ForecastWeatherEndpoint,
                buildQuery: () => _requestBuilder.BuildQueryForForecastWeather(forecastWeatherRequest),
                location: forecastWeatherRequest.Location,
                dataDescription: "forecast weather data");
        }
        #endregion

        #region private methods
        private static string BuildCurrentWeatherCacheKey(GetCurrentWeatherRequest r)
            => $"current|{r.Location}|{r.units}|{r.language}|{r.callback}";

        private static string BuildForecastWeatherCacheKey(GetForecastWeatherRequest r)
            => $"forecast|{r.Location}|{r.forecastDays}|{r.hourly}|{r.interval}|{r.units}|{r.language}|{r.callback}";

        private async Task<ServiceWrapper<T>> GetWeatherAsync<T>(
            string cacheKey,
            string endpoint,
            Func<string> buildQuery,
            string location,
            string dataDescription) where T : class, IWeatherstackResponse
        {
            if (_memoryCache.TryGetValue(cacheKey, out ServiceWrapper<T>? cachedResponse) && cachedResponse != null)
            {
                _logger.LogInformation("Returning cached {DataDescription} for location: {Location}", dataDescription, location);
                return cachedResponse;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("WeatherServiceClient");
                var requestUrl = $"{WeatherApiBaseUrl}/{endpoint}?access_key={WeatherApiKey}&query={buildQuery()}";

                _logger.LogInformation("Requested {DataDescription} for location: {Location}", dataDescription, location);
                var response = await httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return Failure<T>(MapStatusCodeErrorKind(response.StatusCode), $"Weatherstack API returned status code {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();

                T? weatherResponse;
                try
                {
                    weatherResponse = JsonSerializer.Deserialize<T>(content);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Could not deserialize {DataDescription} for location: {Location}", dataDescription, location);
                    return Failure<T>(ServiceErrorKind.UpstreamError, "The weather provider returned a response that could not be processed.");
                }

                // Weatherstack reports application-level failures (invalid key, unknown location,
                // exhausted quota) with HTTP 200 and success:false, so the status code alone is not enough.
                if (weatherResponse is null || weatherResponse.success == false || weatherResponse.error is not null)
                {
                    var providerError = weatherResponse?.error;
                    _logger.LogWarning(
                        "Weatherstack reported an error for location {Location}: {ErrorCode} {ErrorType} {ErrorInfo}",
                        location, providerError?.code, providerError?.type, providerError?.info);

                    return Failure<T>(MapProviderErrorKind(providerError?.code), BuildProviderErrorMessage(providerError));
                }

                if (!weatherResponse.HasUsablePayload)
                {
                    _logger.LogWarning("Weatherstack returned an incomplete payload for location: {Location}", location);
                    return Failure<T>(ServiceErrorKind.UpstreamError, "The weather provider returned an incomplete response.");
                }

                _logger.LogInformation("Successfully fetched {DataDescription} for location: {Location}", dataDescription, location);

                var result = new ServiceWrapper<T>
                {
                    IsSuccess = true,
                    Data = weatherResponse,
                    DetailedErrorMessage = null,
                    ErrorKind = ServiceErrorKind.None
                };

                _memoryCache.Set(cacheKey, result, CacheDuration);
                _logger.LogInformation("Cached {DataDescription} for location: {Location} for {CacheDuration} minutes", dataDescription, location, CacheDuration.TotalMinutes);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching {DataDescription}", dataDescription);
                return Failure<T>(ServiceErrorKind.UpstreamUnavailable, $"An error occurred while fetching {dataDescription}.");
            }
        }

        /// <summary>
        /// Throttling and gateway-level responses are transient, so they are reported as
        /// "try again later" rather than as a permanent upstream error.
        /// </summary>
        private static ServiceErrorKind MapStatusCodeErrorKind(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.TooManyRequests
                or HttpStatusCode.RequestTimeout
                or HttpStatusCode.BadGateway
                or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout => ServiceErrorKind.UpstreamUnavailable,
            _ => ServiceErrorKind.UpstreamError
        };

        /// <summary>
        /// Weatherstack uses the 6xx range for problems with the submitted query and
        /// the 1xx range for account-level problems (invalid key, exhausted quota).
        /// </summary>
        private static ServiceErrorKind MapProviderErrorKind(int? errorCode) => errorCode switch
        {
            >= 600 and < 700 => ServiceErrorKind.InvalidRequest,
            _ => ServiceErrorKind.UpstreamError
        };

        private static string BuildProviderErrorMessage(WeatherstackError? error) => error is null
            ? "The weather provider rejected the request."
            : $"Weatherstack error {error.code} ({error.type}): {error.info}";

        private static ServiceWrapper<T> Failure<T>(ServiceErrorKind errorKind, string message) where T : class => new()
        {
            IsSuccess = false,
            Data = null,
            DetailedErrorMessage = message,
            ErrorKind = errorKind
        };
        #endregion
    }
}
