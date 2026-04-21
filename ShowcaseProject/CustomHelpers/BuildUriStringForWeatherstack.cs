using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using System.Text;

namespace ShowcaseProject.RestApi.CustomHelpers
{
    /// <summary>
    /// Helper class for building URI query strings for Weatherstack API requests.
    /// </summary>
    public class BuildUriStringForWeatherstack
    {
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
                throw new InvalidOperationException("Failed to build URI for current weather", ex);
            }
        }

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
                throw new InvalidOperationException("Failed to build URI for forecast weather", ex);
            }
        }
    }
}
