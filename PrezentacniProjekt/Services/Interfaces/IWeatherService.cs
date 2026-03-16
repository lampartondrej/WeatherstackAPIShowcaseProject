using PrezentacniProjekt.Shared.Model.DTOs.Shared;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response;

namespace PrezentacniProjekt.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<(CurrentWeatherResponse?, DetailedErrorMessage?)> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest);
        Task<(ForecastWeatherResponse?, DetailedErrorMessage?)> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest);
    }
}
