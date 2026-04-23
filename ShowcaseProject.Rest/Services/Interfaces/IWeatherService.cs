using ShowcaseProject.Shared.Model.DTOs.Shared;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.Wrapper;

namespace ShowcaseProject.Services.Interfaces
{
    /// <summary>
    /// Service interface for retrieving weather information from Weatherstack API.
    /// </summary>
    public interface IWeatherService
    {
        Task<ServiceWrapper<CurrentWeatherResponse>> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest);
        Task<ServiceWrapper<ForecastWeatherResponse>> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest);
    }
}
