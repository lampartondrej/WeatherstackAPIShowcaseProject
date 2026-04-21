using Microsoft.AspNetCore.Mvc;
using ShowcaseProject.Controllers;
using ShowcaseProject.Services.Interfaces;
using ShowcaseProject.Shared.Model.DTOs.Shared;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.Wrapper;

namespace ShowcaseProject.RestApi.Controllers
{
    /// <summary>
    /// Controller for managing weather-related operations.
    /// Provides endpoints for retrieving current weather and weather forecasts.
    /// </summary>
    public class WeatherController : ShowcaseProjectBaseController
    {
        private readonly ILogger<ShowcaseProjectBaseController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(ILogger<ShowcaseProjectBaseController> logger,
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
                        var errorMessage = new DetailedErrorMessage
                        {
                            Message = "An error occurred while fetching current weather data.",
                            Details = response.DetailedErrorMessage ?? "No additional information is available.",
                            HttpStatusCode = StatusCodes.Status400BadRequest
                        };
                        return BadRequest(errorMessage);
                    }
                }
                else
                {
                    var errorMessage = new DetailedErrorMessage
                    {
                        Message = "Invalid request model.",
                        Details = "The provided request did not pass validation.",
                        HttpStatusCode = StatusCodes.Status400BadRequest
                    };
                    return BadRequest(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching current weather data");
                var errorMessage = new DetailedErrorMessage
                {
                    Message = "An unexpected error occurred while processing the request.",
                    Details = "No additional information is available.",
                    HttpStatusCode = StatusCodes.Status500InternalServerError
                };
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }

        [HttpPost]
        [Route("forecast")]
        [ProducesResponseType(typeof(CurrentWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetForecastWeather(GetForecastWeatherRequest getForecastWeatherRequest)
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
                        var errorMessage = new DetailedErrorMessage
                        {
                            Message = "An error occurred while fetching forecast weather data.",
                            Details = response.DetailedErrorMessage ?? "No additional information is available.",
                            HttpStatusCode = StatusCodes.Status400BadRequest
                        };
                        return BadRequest(errorMessage);
                    }
                }
                else
                {
                    var errorMessage = new DetailedErrorMessage
                    {
                        Message = "Invalid request model.",
                        Details = "The provided request did not pass validation.",
                        HttpStatusCode = StatusCodes.Status400BadRequest
                    };
                    return BadRequest(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching forecast weather data");
                var errorMessage = new DetailedErrorMessage
                {
                    Message = "An unexpected error occurred while processing the request.",
                    Details = "No additional information is available.",
                    HttpStatusCode = StatusCodes.Status500InternalServerError
                };
                return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
            }
        }
    }
}
