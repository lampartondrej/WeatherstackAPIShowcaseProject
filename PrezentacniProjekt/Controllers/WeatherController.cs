using Microsoft.AspNetCore.Mvc;
using PrezentacniProjekt.Controllers;
using PrezentacniProjekt.Services.Interfaces;
using PrezentacniProjekt.Shared.Model.DTOs.Shared;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;

namespace PrezentacniProjekt.RestApi.Controllers
{
    public class WeatherController : PrezentacniProjektBaseController
    {
        private readonly ILogger<PrezentacniProjektBaseController> _logger;
        private readonly IWeatherService _weatherService;
        public WeatherController(ILogger<PrezentacniProjektBaseController> logger,
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
