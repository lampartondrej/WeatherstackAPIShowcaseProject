namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents the weather forecast response for a specific date (2026-03-15).
    /// </summary>
    public class ForecastWeatherResponse_20260315
    {
        /// <summary>
        /// Gets or sets the forecast date in string format.
        /// </summary>
        public string? date { get; set; }

        /// <summary>
        /// Gets or sets the forecast date in epoch time format.
        /// </summary>
        public int? date_epoch { get; set; }

        /// <summary>
        /// Gets or sets the astronomical data (sunrise, sunset, moon phase, etc.).
        /// </summary>
        public ForecastWeatherResponseAstro? astro { get; set; }

        /// <summary>
        /// Gets or sets the minimum temperature for the day.
        /// </summary>
        public int? mintemp { get; set; }

        /// <summary>
        /// Gets or sets the maximum temperature for the day.
        /// </summary>
        public int? maxtemp { get; set; }

        /// <summary>
        /// Gets or sets the average temperature for the day.
        /// </summary>
        public int? avgtemp { get; set; }

        /// <summary>
        /// Gets or sets the total snowfall amount.
        /// </summary>
        public int? totalsnow { get; set; }

        /// <summary>
        /// Gets or sets the number of sunshine hours.
        /// </summary>
        public float? sunhour { get; set; }

        /// <summary>
        /// Gets or sets the UV index value.
        /// </summary>
        public int? uv_index { get; set; }

        /// <summary>
        /// Gets or sets the air quality measurements.
        /// </summary>
        public ForecastWeatherResponseAir_Quality? air_quality { get; set; }
    }
}
