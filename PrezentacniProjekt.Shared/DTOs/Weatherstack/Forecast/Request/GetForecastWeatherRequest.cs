using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Request
{
    /// <summary>
    /// Represents a request to get forecast weather data from Weatherstack API.
    /// </summary>
    public class GetForecastWeatherRequest
    {
        /// <summary>
        /// Gets or sets the location for which to retrieve the weather forecast.
        /// This can be a city name, coordinates, or other location identifier.
        /// </summary>
        public required string Location { get; set; }

        /// <summary>
        /// Gets or sets the number of days to forecast.
        /// Optional parameter to specify forecast duration.
        /// </summary>
        public int? forecastDays { get; set; }

        /// <summary>
        /// Gets or sets the hourly forecast interval.
        /// Optional parameter to specify hourly data granularity.
        /// </summary>
        public int? hourly { get; set; }

        /// <summary>
        /// Gets or sets the interval between forecast data points.
        /// Optional parameter to control data frequency.
        /// </summary>
        public int? interval { get; set; }

        /// <summary>
        /// Gets or sets the unit system for the weather data (e.g., 'm' for metric, 'f' for Fahrenheit).
        /// Optional parameter to specify measurement units.
        /// </summary>
        public string? units { get; set; }

        /// <summary>
        /// Gets or sets the language code for the response data.
        /// Optional parameter to specify response language.
        /// </summary>
        public string? language { get; set; }

        /// <summary>
        /// Gets or sets the callback function name for JSONP requests.
        /// Optional parameter for cross-domain requests.
        /// </summary>
        public string? callback { get; set; }
    }
}