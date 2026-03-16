using PrezentacniProjekt.RestApi.CustomHelpers;
using PrezentacniProjekt.Services.Interfaces;
using PrezentacniProjekt.Shared.Model.DTOs.Shared;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response;

namespace PrezentacniProjekt.Services
{
    public class WeatherService : IPrezentacniProjectBaseService, IWeatherService
    {
        private readonly ILogger<IPrezentacniProjectBaseService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string WeatherApiKey;
        private readonly string WeatherstackApiKeyEnvVar;
        private readonly string WeatherApiBaseUrl;
        private readonly string CurrentWeatherEndpoint;
        private readonly string ForecastWeatherEndpoint;
        public WeatherService(ILogger<IPrezentacniProjectBaseService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory) : base(logger, configuration, httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            WeatherstackApiKeyEnvVar = _configuration.GetValue<string>("APIOptions:WeatherstackApiKeyEnvVar") ?? throw new InvalidOperationException("Weather API key environment variable name is not set in configuration.");
            WeatherApiKey = Environment.GetEnvironmentVariable($"{WeatherstackApiKeyEnvVar}") ?? throw new InvalidOperationException($"Weather API key is not set in environment variable '{WeatherstackApiKeyEnvVar}'.");
            WeatherApiBaseUrl = _configuration.GetValue<string>("APIOptions:WeatherstackApiUrl") ?? throw new InvalidOperationException("Weather API base URL is not set in configuration.");
            CurrentWeatherEndpoint = _configuration.GetValue<string>("APIOptions:CurrentWeatherEndpoint") ?? throw new InvalidOperationException("Current weather endpoint is not set in configuration.");
            ForecastWeatherEndpoint = _configuration.GetValue<string>("APIOptions:ForecastWeatherEndpoint") ?? throw new InvalidOperationException("Forecast weather endpoint is not set in configuration.");

        }
        #region public methods
        public async Task<(CurrentWeatherResponse?, DetailedErrorMessage?)> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest)
        {
            return await GetCurrentWeatherAsync(currentWeatherRequest);
        }

        public async Task<(ForecastWeatherResponse?, DetailedErrorMessage?)> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest)
        {
            return await GetForecastWeatherAsync(forecastWeatherRequest);
        }
        #endregion
        #region private methods
        private async Task<(CurrentWeatherResponse?, DetailedErrorMessage?)> GetCurrentWeatherAsync(GetCurrentWeatherRequest currentWeatherRequest)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
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
                    return (weatherResponse, null);
                }
                else
                {
                    var errorMessage = new DetailedErrorMessage
                    {
                        Message = "Failed to fetch current weather data.",
                        Details = $"API returned status code {response.StatusCode}",
                        HttpStatusCode = (int)response.StatusCode
                    };
                    return (null, errorMessage);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching current weather data.");
                var errorMessage = new DetailedErrorMessage
                {
                    Message = "An error occurred while fetching current weather data.",
                    Details = ex.Message,
                    HttpStatusCode = 999
                };
                return (null, errorMessage);
            }
        }

        private async Task<(ForecastWeatherResponse?, DetailedErrorMessage?)> GetForecastWeatherAsync(GetForecastWeatherRequest forecastWeatherRequest)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
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
                    return (weatherResponse, null);
                }
                else
                {
                    var errorMessage = new DetailedErrorMessage
                    {
                        Message = "Failed to fetch forecast weather data.",
                        Details = $"API returned status code {response.StatusCode}",
                        HttpStatusCode = (int)response.StatusCode
                    };
                    return (null, errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching forecast weather data.");
                var errorMessage = new DetailedErrorMessage
                {
                    Message = "An error occurred while fetching forecast weather data.",
                    Details = ex.Message,
                    HttpStatusCode = 999
                };
                return (null, errorMessage);
            }
        }
        #endregion
    }
}