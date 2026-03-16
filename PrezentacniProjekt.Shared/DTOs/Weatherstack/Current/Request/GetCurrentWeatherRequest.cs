using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request
{
    /// <summary>
    /// Represents a request to get current weather information from Weatherstack API.
    /// </summary>
    public class GetCurrentWeatherRequest
    {
        /// <summary>
        /// Gets or sets the location for which to retrieve weather data.
        /// This can be a city name, coordinates, IP address, or other location identifier.
        /// </summary>
        public required string Location { get; set; }

        /// <summary>
        /// Gets or sets the unit system for the response (e.g., "m" for metric, "f" for Fahrenheit, "s" for scientific).
        /// This parameter is optional.
        /// </summary>
        public string? units { get; set; }

        /// <summary>
        /// Gets or sets the language code for the response (e.g., "en" for English, "de" for German).
        /// This parameter is optional.
        /// </summary>
        public string? language { get; set; }

        /// <summary>
        /// Gets or sets the callback function name for JSONP requests.
        /// This parameter is optional.
        /// </summary>
        public string? callback { get; set; }
    }
}
