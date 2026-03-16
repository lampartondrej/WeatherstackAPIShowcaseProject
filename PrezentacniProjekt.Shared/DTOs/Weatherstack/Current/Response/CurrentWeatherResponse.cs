using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    /// <summary>
    /// Represents the response from the Weatherstack current weather API.
    /// </summary>
    public class CurrentWeatherResponse
    {
        /// <summary>
        /// Gets or sets the request information that was used to retrieve the weather data.
        /// </summary>
        public CurrentWeatherResponseRequest? request { get; set; }

        /// <summary>
        /// Gets or sets the location information for the weather data.
        /// </summary>
        public CurrentWeatherResponseLocation? location { get; set; }

        /// <summary>
        /// Gets or sets the current weather conditions and measurements.
        /// </summary>
        public CurrentWeatherResponseCurrent? current { get; set; }
    }
}
