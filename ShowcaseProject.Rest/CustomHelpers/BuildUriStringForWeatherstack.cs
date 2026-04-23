using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.RestApi.CustomHelpers;
using System.Text;

namespace ShowcaseProject.RestApi.CustomHelpers
{
    /// <summary>
    /// Builds URL-encoded query strings for Weatherstack API requests.
    /// </summary>
    public class BuildUriStringForWeatherstack : IWeatherstackRequestBuilder
    {
        private const int EstimatedQueryStringLength = 128;

        public string BuildQueryForCurrentWeather(GetCurrentWeatherRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Location);

            var query = new StringBuilder(EstimatedQueryStringLength);
            query.Append(Uri.EscapeDataString(request.Location));

            AppendParameter(query, "units", request.units);
            AppendParameter(query, "language", request.language);
            AppendParameter(query, "callback", request.callback);

            return query.ToString();
        }

        public string BuildQueryForForecastWeather(GetForecastWeatherRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentException.ThrowIfNullOrWhiteSpace(request.Location);

            var query = new StringBuilder(EstimatedQueryStringLength);
            query.Append(Uri.EscapeDataString(request.Location));

            AppendParameter(query, "forecast_days", request.forecastDays);
            AppendParameter(query, "hourly", request.hourly);
            AppendParameter(query, "interval", request.interval);
            AppendParameter(query, "units", request.units);
            AppendParameter(query, "language", request.language);
            AppendParameter(query, "callback", request.callback);

            return query.ToString();
        }

        private static void AppendParameter(StringBuilder queryString, string parameterName, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                queryString.Append('&')
                           .Append(parameterName)
                           .Append('=')
                           .Append(Uri.EscapeDataString(value));
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
