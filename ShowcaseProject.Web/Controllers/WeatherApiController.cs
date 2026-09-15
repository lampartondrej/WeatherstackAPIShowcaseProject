using Microsoft.AspNetCore.Mvc;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShowcaseProject.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherApiController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherApiController> _logger;

        public WeatherApiController(IHttpClientFactory httpClientFactory, ILogger<WeatherApiController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet("current/{location}")]
        public Task<IActionResult> GetCurrentWeather(string location)
        {
            return ForwardToApiAsync<GetCurrentWeatherRequest, CurrentWeatherResponse>(
                "/Weather/current",
                new GetCurrentWeatherRequest { Location = location },
                location,
                "current weather");
        }

        [HttpGet("forecast/{location}")]
        public Task<IActionResult> GetForecastWeather(string location)
        {
            return ForwardToApiAsync<GetForecastWeatherRequest, ForecastWeatherResponse>(
                "/Weather/forecast",
                new GetForecastWeatherRequest { Location = location },
                location,
                "forecast weather");
        }

        private async Task<IActionResult> ForwardToApiAsync<TRequest, TResponse>(
            string endpoint,
            TRequest request,
            string location,
            string dataDescription)
        {
            try
            {
                // Authorization is attached by BasicAuthenticationHeaderHandler on the named client.
                var client = _httpClientFactory.CreateClient("WeatherApi");
                var response = await client.PostAsJsonAsync(endpoint, request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch {DataDescription}. Status: {StatusCode}", dataDescription, response.StatusCode);
                    return StatusCode((int)response.StatusCode, $"Failed to fetch {dataDescription} data");
                }

                var weatherData = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {DataDescription} for location: {Location}", dataDescription, location);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while fetching {dataDescription} data");
            }
        }
    }
}
