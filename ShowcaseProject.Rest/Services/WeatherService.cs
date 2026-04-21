using ShowcaseProject.RestApi.CustomHelpers;
using Microsoft.Extensions.Caching.Memory;
using ShowcaseProject.Services.Interfaces;
using ShowcaseProject.Shared.Model.DTOs.Shared;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.Wrapper;

namespace ShowcaseProject.Services
{
    /// <summary>
    /// Service implementation for retrieving weather information from the Weatherstack API.
    /// </summary>
    public class WeatherService : IShowcaseProjectBaseService, IWeatherService
    {
        private readonly ILogger<IShowcaseProjectBaseService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly string WeatherApiKey;
        private readonly string WeatherstackApiKeyEnvVar;
        private readonly string WeatherApiBaseUrl;
        private readonly string CurrentWeatherEndpoint;
        private readonly string ForecastWeatherEndpoint;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public WeatherService(ILogger<IShowcaseProjectBaseService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache memoryCache) : base(logger, configuration, httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            WeatherstackApiKeyEnvVar = _configuration.GetValue<string>("APIOptions:WeatherstackApiKeyEnvVar") ?? throw new InvalidOperationException("Weather API key environment variable name is not set in configuration.");
            WeatherApiKey = Environment.GetEnvironmentVariable($"{WeatherstackApiKeyEnvVar}") ?? throw new InvalidOperationException($"Weather API key is not set in environment variable '{WeatherstackApiKeyEnvVar}'.");
            WeatherApiBaseUrl = _configuration.GetValue<string>("APIOptions:WeatherstackApiUrl") ?? throw new InvalidOperationException("Weather API base URL is not set in configuration.");
            CurrentWeatherEndpoint = _configuration.GetValue<string>("APIOptions:CurrentWeatherEndpoint") ?? throw new InvalidOperationException("Current weather endpoint is not set in configuration.");
            ForecastWeatherEndpoint = _configuration.GetValue<string>("APIOptions:ForecastWeatherEndpoint") ?? throw new InvalidOperationException("Forecast weather endpoint is not set in configuration.");

        }
        #region public methods
        public async Task<ServiceWrapper<CurrentWeatherResponse>> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest)
        {
            return await GetCurrentWeatherAsync(currentWeatherRequest);
        }

        public async Task<ServiceWrapper<ForecastWeatherResponse>> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest)
        {
            return await GetForecastWeatherAsync(forecastWeatherRequest);
        }
        #endregion
        #region private methods
        private async Task<ServiceWrapper<CurrentWeatherResponse>> GetCurrentWeatherAsync(GetCurrentWeatherRequest currentWeatherRequest)
        {
            var cacheKey = $"current_weather_{currentWeatherRequest.Location}";

            if (_memoryCache.TryGetValue(cacheKey, out ServiceWrapper<CurrentWeatherResponse>? cachedResponse) && cachedResponse != null)
            {
                _logger.LogInformation("Returning cached current weather data for location: {Location}", currentWeatherRequest.Location);
                return cachedResponse;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient("WeatherServiceClient");
                var buildUriHelper = new BuildUriStringForWeatherstack();
                var queryString = buildUriHelper.BuildUriForCurrentWeather(currentWeatherRequest);
                var requestUrl = $"{WeatherApiBaseUrl}/{CurrentWeatherEndpoint}?access_key={WeatherApiKey}&query={queryString}";

                var response = await httpClient.GetAsync(requestUrl);
                _logger.LogInformation("Requested current weather data with URL: {RequestUrl}", requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherResponse = System.Text.Json.JsonSerializer.Deserialize<CurrentWeatherResponse>(content);
                    _logger.LogInformation("Successfully fetched current weather data for location: {Location}", currentWeatherRequest.Location);
                    var result = new ServiceWrapper<CurrentWeatherResponse>
                    {
                        IsSuccess = true,
                        Data = weatherResponse,
                        DetailedErrorMessage = null
                    };

                    if (weatherResponse != null)
                    {
                        _memoryCache.Set(cacheKey, result, CacheDuration);
                        _logger.LogInformation("Cached current weather data for location: {Location} for {CacheDuration} minutes", currentWeatherRequest.Location, CacheDuration.TotalMinutes);
                    }

                    return result;
                }
                else
                {
                    var errorMessage = $"Weatherstack API returned status code {response.StatusCode}";
                    return new ServiceWrapper<CurrentWeatherResponse>
                    {
                        IsSuccess = false,
                        Data = null,
                        DetailedErrorMessage = errorMessage
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching current weather data. {ex.Message}, {ex.InnerException}");
                var errorMessage = $"An error occurred while fetching current weather data.";
                return new ServiceWrapper<CurrentWeatherResponse>
                {
                    IsSuccess = false,
                    Data = null,
                    DetailedErrorMessage = errorMessage
                };
            }
        }

        private async Task<ServiceWrapper<ForecastWeatherResponse>> GetForecastWeatherAsync(GetForecastWeatherRequest forecastWeatherRequest)
        {
            var cacheKey = $"forecast_weather_{forecastWeatherRequest.Location}";

            if (_memoryCache.TryGetValue(cacheKey, out ServiceWrapper<ForecastWeatherResponse>? cachedResponse) && cachedResponse != null)
            {
                _logger.LogInformation("Returning cached forecast weather data for location: {Location}", forecastWeatherRequest.Location);
                return cachedResponse;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient("WeatherServiceClient");
                var buildUriHelper = new BuildUriStringForWeatherstack();
                var queryString = buildUriHelper.BuildUriForForecastWeather(forecastWeatherRequest);
                var requestUrl = $"{WeatherApiBaseUrl}/{ForecastWeatherEndpoint}?access_key={WeatherApiKey}&query={queryString}";

                var response = await httpClient.GetAsync(requestUrl);
                _logger.LogInformation("Requested forecast weather data with URL: {RequestUrl}", requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherResponse = System.Text.Json.JsonSerializer.Deserialize<ForecastWeatherResponse>(content);
                    _logger.LogInformation("Successfully fetched forecast weather data for location: {Location}", forecastWeatherRequest.Location);
                    var result = new ServiceWrapper<ForecastWeatherResponse>
                    {
                        IsSuccess = true,
                        Data = weatherResponse,
                        DetailedErrorMessage = null
                    };

                    if (weatherResponse != null)
                    {
                        _memoryCache.Set(cacheKey, result, CacheDuration);
                        _logger.LogInformation("Cached forecast weather data for location: {Location} for {CacheDuration} minutes", forecastWeatherRequest.Location, CacheDuration.TotalMinutes);
                    }

                    return result;
                }
                else
                {
                    var errorMessage = $"Weatherstack API returned status code {response.StatusCode}";
                    return new ServiceWrapper<ForecastWeatherResponse>
                    {
                        IsSuccess = false,
                        Data = null,
                        DetailedErrorMessage = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching forecast weather data. {ex.Message}, {ex.InnerException}");
                var errorMessage = $"An error occurred while fetching forecast weather data.";
                return new ServiceWrapper<ForecastWeatherResponse>
                {
                    IsSuccess = false,
                    Data = null,
                    DetailedErrorMessage = errorMessage
                };
            }
        }
        #endregion
    }
}