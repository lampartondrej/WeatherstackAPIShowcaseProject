using Microsoft.AspNetCore.Mvc;
using ShowcaseProject.Controllers;
using ShowcaseProject.Services.Interfaces;
using ShowcaseProject.Shared.Model.DTOs.Shared;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.Wrapper;

namespace ShowcaseProject.RestApi.Controllers
{
    /// <summary>
    /// Controller for managing weather-related operations.
    /// Provides endpoints for retrieving current weather and weather forecasts.
    /// </summary>
    public class WeatherController : ShowcaseProjectBaseController
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(ILogger<WeatherController> logger,
            IWeatherService weatherService) : base(logger)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpPost]
        [Route("current")]
        [ProducesResponseType(typeof(CurrentWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentWeather([FromBody] GetCurrentWeatherRequest currentWeatherRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _weatherService.GetCurrentWeather(currentWeatherRequest);
                    if (response.IsSuccess && response.Data != null)
                    {
                        return Ok(response.Data);
                    }
                    else
                    {
                        return BadRequest(ServiceError("An error occurred while fetching current weather data.", response.DetailedErrorMessage));
                    }
                }
                else
                {
                    return BadRequest(ServiceError("Invalid request model.", "The provided request did not pass validation."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching current weather data");
                return StatusCode(StatusCodes.Status500InternalServerError, UnexpectedError());
            }
        }

        [HttpPost]
        [Route("forecast")]
        [ProducesResponseType(typeof(ForecastWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetForecastWeather([FromBody] GetForecastWeatherRequest getForecastWeatherRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _weatherService.GetForecastWeather(getForecastWeatherRequest);
                    if (response.IsSuccess && response.Data != null)
                    {
                        return Ok(response.Data);
                    }
                    else
                    {
                        return BadRequest(ServiceError("An error occurred while fetching forecast weather data.", response.DetailedErrorMessage));
                    }
                }
                else
                {
                    return BadRequest(ServiceError("Invalid request model.", "The provided request did not pass validation."));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching forecast weather data");
                return StatusCode(StatusCodes.Status500InternalServerError, UnexpectedError());
            }
        }

        private static DetailedErrorMessage ServiceError(string message, string? details) => new()
        {
            Message = message,
            Details = details ?? "No additional information is available.",
            HttpStatusCode = StatusCodes.Status400BadRequest
        };

        private static DetailedErrorMessage UnexpectedError() => new()
        {
            Message = "An unexpected error occurred while processing the request.",
            Details = "No additional information is available.",
            HttpStatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
