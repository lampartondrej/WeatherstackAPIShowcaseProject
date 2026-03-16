using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents the response from the Weatherstack forecast API.
    /// </summary>
    public class ForecastWeatherResponse
    {
        /// <summary>
        /// Gets or sets the request details used for the weather forecast query.
        /// </summary>
        public ForecastWeatherResponseRequest? request { get; set; }

        /// <summary>
        /// Gets or sets the location information for the weather forecast.
        /// </summary>
        public ForecastWeatherResponseLocation? location { get; set; }

        /// <summary>
        /// Gets or sets the current weather conditions.
        /// </summary>
        public ForecastWeatherResponseCurrent? current { get; set; }

        /// <summary>
        /// Gets or sets the weather forecast data.
        /// </summary>
        public ForecastWeatherResponseForecast? forecast { get; set; }
    }
}
