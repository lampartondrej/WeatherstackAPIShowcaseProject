namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents astronomical data for a specific day in the weather forecast.
    /// </summary>
    public class ForecastWeatherResponseAstro
    {
        /// <summary>
        /// Gets or sets the sunrise time.
        /// </summary>
        public string? sunrise { get; set; }

        /// <summary>
        /// Gets or sets the sunset time.
        /// </summary>
        public string? sunset { get; set; }

        /// <summary>
        /// Gets or sets the moonrise time.
        /// </summary>
        public string? moonrise { get; set; }

        /// <summary>
        /// Gets or sets the moonset time.
        /// </summary>
        public string? moonset { get; set; }

        /// <summary>
        /// Gets or sets the moon phase description.
        /// </summary>
        public string? moon_phase { get; set; }

        /// <summary>
        /// Gets or sets the moon illumination percentage.
        /// </summary>
        public int? moon_illumination { get; set; }
    }
}
