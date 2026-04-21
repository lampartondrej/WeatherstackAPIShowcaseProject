using Microsoft.AspNetCore.Mvc;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShowcaseProject.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherApiController> _logger;
        private readonly string _apiUsername;
        private readonly string _apiPassword;

        public WeatherApiController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<WeatherApiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;

            // Get environment variable names from configuration
            var usernameEnvVar = _configuration.GetValue<string>("AuthSettings:UsernameEnvVar")
                ?? throw new InvalidOperationException("AuthSettings:UsernameEnvVar is not set in configuration.");
            var passwordEnvVar = _configuration.GetValue<string>("AuthSettings:PasswordEnvVar")
                ?? throw new InvalidOperationException("AuthSettings:PasswordEnvVar is not set in configuration.");

            // Get actual credentials from environment variables
            _apiUsername = Environment.GetEnvironmentVariable(usernameEnvVar)
                ?? throw new InvalidOperationException($"Environment variable '{usernameEnvVar}' is not set.");
            _apiPassword = Environment.GetEnvironmentVariable(passwordEnvVar)
                ?? throw new InvalidOperationException($"Environment variable '{passwordEnvVar}' is not set.");
        }

        [HttpGet("current/{location}")]
        public async Task<IActionResult> GetCurrentWeather(string location)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WeatherApi");
                
                // Add Basic Authentication using credentials from environment variables
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiUsername}:{_apiPassword}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var request = new GetCurrentWeatherRequest { Location = location };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("/Weather/current", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonSerializer.Deserialize<CurrentWeatherResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(weatherData);
                }
                else
                {
                    _logger.LogError("Failed to fetch current weather. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Failed to fetch weather data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current weather for location: {Location}", location);
                return StatusCode(500, "An error occurred while fetching weather data");
            }
        }

        [HttpGet("forecast/{location}")]
        public async Task<IActionResult> GetForecastWeather(string location)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("WeatherApi");
                
                // Add Basic Authentication using credentials from environment variables
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_apiUsername}:{_apiPassword}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                var request = new GetForecastWeatherRequest 
                { 
                    Location = location
                };
                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("/Weather/forecast", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonSerializer.Deserialize<ForecastWeatherResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return Ok(weatherData);
                }
                else
                {
                    _logger.LogError("Failed to fetch forecast weather. Status: {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, "Failed to fetch forecast data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching forecast weather for location: {Location}", location);
                return StatusCode(500, "An error occurred while fetching forecast data");
            }
        }
    }
}
