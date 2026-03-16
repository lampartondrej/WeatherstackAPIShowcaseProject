using PrezentacniProjekt.Shared.Model.DTOs.Shared;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response;

namespace PrezentacniProjekt.Services.Interfaces
{
    /// <summary>
    /// Service interface for retrieving weather information from Weatherstack API.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Gets the current weather for a specified location.
        /// </summary>
        /// <param name="currentWeatherRequest">The request containing location and query parameters.</param>
        /// <returns>
        /// A tuple containing either the current weather response or a detailed error message if the request fails.
        /// </returns>
        Task<(CurrentWeatherResponse?, DetailedErrorMessage?)> GetCurrentWeather(GetCurrentWeatherRequest currentWeatherRequest);

        /// <summary>
        /// Gets the forecast weather for a specified location.
        /// </summary>
        /// <param name="forecastWeatherRequest">The request containing location, forecast days, and other query parameters.</param>
        /// <returns>
        /// A tuple containing either the forecast weather response or a detailed error message if the request fails.
        /// </returns>
        Task<(ForecastWeatherResponse?, DetailedErrorMessage?)> GetForecastWeather(GetForecastWeatherRequest forecastWeatherRequest);
    }
}
