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
    /// Weather data sourced from the Weatherstack provider. Every call is authenticated with Basic
    /// authentication, rate limited per client, retried on transient upstream failures and served
    /// from an in-memory cache for 10 minutes when an identical query was already answered.
    /// </summary>
    public class WeatherController : ShowcaseProjectBaseController
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        /// <summary>
        /// Returns the current weather conditions for a single location.
        /// </summary>
        /// <remarks>
        /// The location accepts anything the provider understands — a city name, a postal code,
        /// `latitude,longitude` coordinates, or an IP address. Only the location is required;
        /// the remaining fields control the unit system, the language of the textual descriptions
        /// and an optional JSONP callback.
        ///
        /// Identical queries are answered from the in-memory cache for 10 minutes. Provider errors
        /// are never cached, so a transient failure is retried on the next call.
        ///
        /// Sample request:
        ///
        ///     POST /Weather/current
        ///     {
        ///        "location": "Prague",
        ///        "units": "m",
        ///        "language": "en"
        ///     }
        ///
        /// </remarks>
        /// <param name="currentWeatherRequest">Location to query plus optional formatting options.</param>
        /// <response code="200">Current conditions for the requested location.</response>
        /// <response code="400">The request failed validation, or the provider rejected the location or a parameter.</response>
        /// <response code="401">Basic authentication credentials are missing or invalid.</response>
        /// <response code="429">The client exceeded the configured request rate limit.</response>
        /// <response code="500">An unexpected error occurred while processing the request.</response>
        /// <response code="502">The weather provider returned an error or an unusable payload.</response>
        /// <response code="503">The weather provider could not be reached before the retries were exhausted.</response>
        [HttpPost]
        [Route("current")]
        [ProducesResponseType(typeof(CurrentWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status502BadGateway)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status503ServiceUnavailable)]
        public Task<IActionResult> GetCurrentWeather([FromBody] GetCurrentWeatherRequest currentWeatherRequest)
        {
            return HandleWeatherRequestAsync(
                () => _weatherService.GetCurrentWeather(currentWeatherRequest),
                "An error occurred while fetching current weather data.",
                "current weather data");
        }

        /// <summary>
        /// Returns the weather forecast, together with the current conditions, for a single location.
        /// </summary>
        /// <remarks>
        /// Besides the shared formatting options, this endpoint accepts the number of forecast days,
        /// whether hourly values should be included and the interval those hourly values are reported in.
        ///
        /// Forecast availability depends on the Weatherstack plan in use — on the free tier the provider
        /// rejects the request, which this API surfaces as a `400` with the provider's own error text.
        ///
        /// Sample request:
        ///
        ///     POST /Weather/forecast
        ///     {
        ///        "location": "Prague",
        ///        "forecastDays": 3,
        ///        "hourly": 1,
        ///        "interval": 3,
        ///        "units": "m"
        ///     }
        ///
        /// </remarks>
        /// <param name="getForecastWeatherRequest">Location to query plus forecast and formatting options.</param>
        /// <response code="200">Forecast and current conditions for the requested location.</response>
        /// <response code="400">The request failed validation, or the provider rejected the location or a parameter.</response>
        /// <response code="401">Basic authentication credentials are missing or invalid.</response>
        /// <response code="429">The client exceeded the configured request rate limit.</response>
        /// <response code="500">An unexpected error occurred while processing the request.</response>
        /// <response code="502">The weather provider returned an error or an unusable payload.</response>
        /// <response code="503">The weather provider could not be reached before the retries were exhausted.</response>
        [HttpPost]
        [Route("forecast")]
        [ProducesResponseType(typeof(ForecastWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status502BadGateway)]
        [ProducesResponseType(typeof(DetailedErrorMessage), StatusCodes.Status503ServiceUnavailable)]
        public Task<IActionResult> GetForecastWeather([FromBody] GetForecastWeatherRequest getForecastWeatherRequest)
        {
            return HandleWeatherRequestAsync(
                () => _weatherService.GetForecastWeather(getForecastWeatherRequest),
                "An error occurred while fetching forecast weather data.",
                "forecast weather data");
        }

        private async Task<IActionResult> HandleWeatherRequestAsync<T>(
            Func<Task<ServiceWrapper<T>>> fetchWeather,
            string failureMessage,
            string dataDescription) where T : class
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ServiceError("Invalid request model.", "The provided request did not pass validation.", ServiceErrorKind.InvalidRequest));
                }

                var response = await fetchWeather();
                if (response.IsSuccess && response.Data != null)
                {
                    return Ok(response.Data);
                }

                var error = ServiceError(failureMessage, response.DetailedErrorMessage, response.ErrorKind);
                return StatusCode(error.HttpStatusCode, error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching {DataDescription}", dataDescription);
                return StatusCode(StatusCodes.Status500InternalServerError, UnexpectedError());
            }
        }

        private static DetailedErrorMessage ServiceError(string message, string? details, ServiceErrorKind errorKind) => new()
        {
            Message = message,
            Details = details ?? "No additional information is available.",
            HttpStatusCode = MapErrorKindToStatusCode(errorKind)
        };

        private static int MapErrorKindToStatusCode(ServiceErrorKind errorKind) => errorKind switch
        {
            ServiceErrorKind.InvalidRequest => StatusCodes.Status400BadRequest,
            ServiceErrorKind.UpstreamError => StatusCodes.Status502BadGateway,
            ServiceErrorKind.UpstreamUnavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        private static DetailedErrorMessage UnexpectedError() => new()
        {
            Message = "An unexpected error occurred while processing the request.",
            Details = "No additional information is available.",
            HttpStatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
