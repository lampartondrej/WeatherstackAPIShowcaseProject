using Microsoft.AspNetCore.Mvc;
using PrezentacniProjekt.Controllers;
using PrezentacniProjekt.Services.Interfaces;
using PrezentacniProjekt.Shared.Model.DTOs.Shared;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;

namespace PrezentacniProjekt.RestApi.Controllers
{
    /// <summary>
    /// Controller for managing weather-related operations.
    /// Provides endpoints for retrieving current weather and weather forecasts.
    /// </summary>
    public class WeatherController : PrezentacniProjektBaseController
    {
        private readonly ILogger<PrezentacniProjektBaseController> _logger;
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="weatherService">The weather service for retrieving weather data.</param>
        public WeatherController(ILogger<PrezentacniProjektBaseController> logger,
            IWeatherService weatherService) : base(logger)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        /// <summary>
        /// Retrieves current weather information for a specified location.
        /// </summary>
        /// <param name="currentWeatherRequest">The request containing location and query parameters.</param>
        /// <returns>Current weather data or error information.</returns>
        /// <response code="200">Returns the current weather data.</response>
        /// <response code="400">Returns error details if the request is invalid.</response>
        /// <response code="500">Returns error details if an internal server error occurs.</response>
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
                    if (response.Item1 != null)
                    {
                        return Ok(response.Item1);
                    }
                    else if (response.Item2 != null)
                    {
                        return BadRequest(response.Item2);
                    }
                    else
                    {
                        var errorMessage = new DetailedErrorMessage
                        {
                            Message = "An unexpected error occurred while processing the request.",
                            Details = "No data was returned from the weather service.",
                            HttpStatusCode = StatusCodes.Status500InternalServerError
                        };
                        return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
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

        /// <summary>
        /// Retrieves weather forecast information for a specified location.
        /// </summary>
        /// <param name="getForecastWeatherRequest">The request containing location and forecast parameters.</param>
        /// <returns>Weather forecast data or error information.</returns>
        /// <response code="200">Returns the weather forecast data.</response>
        /// <response code="400">Returns error details if the request is invalid.</response>
        /// <response code="500">Returns error details if an internal server error occurs.</response>
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
                    if (response.Item1 != null)
                    {
                        return Ok(response.Item1);
                    }
                    else if (response.Item2 != null)
                    {
                        return BadRequest(response.Item2);
                    }
                    else
                    {
                        var errorMessage = new DetailedErrorMessage
                        {
                            Message = "An unexpected error occurred while processing the request.",
                            Details = "No data was returned from the weather service.",
                            HttpStatusCode = StatusCodes.Status500InternalServerError
                        };
                        return StatusCode(StatusCodes.Status500InternalServerError, errorMessage);
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
