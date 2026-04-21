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
        private const int EstimatedQueryStringLength = 128;

        public string BuildUriForCurrentWeather(GetCurrentWeatherRequest currentRequest)
        {
            ArgumentNullException.ThrowIfNull(currentRequest);
            ArgumentException.ThrowIfNullOrWhiteSpace(currentRequest.Location);

            var queryString = new StringBuilder(EstimatedQueryStringLength);
            queryString.Append(currentRequest.Location);

            AppendParameter(queryString, "units", currentRequest.units);
            AppendParameter(queryString, "language", currentRequest.language);
            AppendParameter(queryString, "callback", currentRequest.callback);

            return queryString.ToString();
        }

        public string BuildUriForForecastWeather(GetForecastWeatherRequest forecastRequest)
        {
            ArgumentNullException.ThrowIfNull(forecastRequest);
            ArgumentException.ThrowIfNullOrWhiteSpace(forecastRequest.Location);

            var queryString = new StringBuilder(EstimatedQueryStringLength);
            queryString.Append(forecastRequest.Location);

            AppendParameter(queryString, "forecast_days", forecastRequest.forecastDays);
            AppendParameter(queryString, "hourly", forecastRequest.hourly);
            AppendParameter(queryString, "interval", forecastRequest.interval);
            AppendParameter(queryString, "units", forecastRequest.units);
            AppendParameter(queryString, "language", forecastRequest.language);
            AppendParameter(queryString, "callback", forecastRequest.callback);

            return queryString.ToString();
        }

        private static void AppendParameter(StringBuilder queryString, string parameterName, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                queryString.Append('&')
                           .Append(parameterName)
                           .Append('=')
                           .Append(value);
            }
        }

        private static void AppendParameter(StringBuilder queryString, string parameterName, int? value)
        {
            if (value.HasValue)
            {
                queryString.Append('&')
                           .Append(parameterName)
                           .Append('=')
                           .Append(value.Value);
            }
        }
    }
}
