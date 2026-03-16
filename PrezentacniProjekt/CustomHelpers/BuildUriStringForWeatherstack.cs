using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using System.Text;

namespace PrezentacniProjekt.RestApi.CustomHelpers
{
    /// <summary>
    /// Helper class for building URI query strings for Weatherstack API requests.
    /// </summary>
    public class BuildUriStringForWeatherstack
    {
        /// <summary>
        /// Builds a URI query string for current weather API requests.
        /// </summary>
        /// <param name="currentRequest">The current weather request containing location and optional parameters.</param>
        /// <returns>A formatted query string with location and optional parameters.</returns>
        /// <exception cref="InvalidOperationException">Thrown when URI building fails.</exception>
        public string BuildUriForCurrentWeather(GetCurrentWeatherRequest currentRequest)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.Append($"{currentRequest.Location}");
                if (!string.IsNullOrEmpty(currentRequest.units))
                {
                    str.Append($"&units={currentRequest.units}");
                }
                if (!string.IsNullOrEmpty(currentRequest.language))
                {
                    str.Append($"&language={currentRequest.language}");
                }
                if (!string.IsNullOrEmpty(currentRequest.callback))
                {
                    str.Append($"&callback={currentRequest.callback}");
                }
                return str.ToString();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new InvalidOperationException("Failed to build URI for current weather", ex);
            }
        }

        /// <summary>
        /// Builds a URI query string for forecast weather API requests.
        /// </summary>
        /// <param name="forecastRequest">The forecast weather request containing location and optional parameters.</param>
        /// <returns>A formatted query string with location and optional forecast parameters.</returns>
        /// <exception cref="InvalidOperationException">Thrown when URI building fails.</exception>
        public string BuildUriForForecastWeather(GetForecastWeatherRequest forecastRequest)
        {
            try
            {
                StringBuilder str = new StringBuilder();
                str.Append(forecastRequest.Location);
                if (forecastRequest.forecastDays.HasValue)
                {
                    str.Append($"&forecast_days={forecastRequest.forecastDays.Value}");
                }
                if (forecastRequest.hourly.HasValue)
                {
                    str.Append($"&hourly={forecastRequest.hourly.Value}");
                }
                if (forecastRequest.interval.HasValue)
                {
                    str.Append($"&interval={forecastRequest.interval.Value}");
                }
                if (!string.IsNullOrEmpty(forecastRequest.units))
                {
                    str.Append($"&units={forecastRequest.units}");
                }
                if (!string.IsNullOrEmpty(forecastRequest.language))
                {
                    str.Append($"&language={forecastRequest.language}");
                }
                if (!string.IsNullOrEmpty(forecastRequest.callback))
                {
                    str.Append($"&callback={forecastRequest.callback}");
                }
                return str.ToString();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new InvalidOperationException("Failed to build URI for forecast weather", ex);

            }
        }
    }
}
