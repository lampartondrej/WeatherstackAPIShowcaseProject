using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;

namespace ShowcaseProject.RestApi.CustomHelpers
{
    /// <summary>
    /// Builds query strings for Weatherstack API requests.
    /// </summary>
    public interface IWeatherstackRequestBuilder
    {
        string BuildQueryForCurrentWeather(GetCurrentWeatherRequest request);
        string BuildQueryForForecastWeather(GetForecastWeatherRequest request);
    }
}
