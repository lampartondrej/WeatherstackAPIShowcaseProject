using System.ComponentModel.DataAnnotations;

namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request
{
    /// <summary>
    /// Query for the weather forecast at a single location.
    /// </summary>
    public class GetForecastWeatherRequest
    {
        /// <summary>
        /// Location to look up: a city name, postal code, "latitude,longitude" pair or IP address.
        /// </summary>
        /// <example>Prague</example>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Location { get; set; }

        /// <summary>
        /// Number of days to forecast. Availability depends on the provider plan in use.
        /// </summary>
        /// <example>3</example>
        public int? forecastDays { get; set; }

        /// <summary>
        /// Set to 1 to include hourly values in the forecast, 0 to omit them.
        /// </summary>
        /// <example>1</example>
        public int? hourly { get; set; }

        /// <summary>
        /// Interval of the hourly values in hours (1, 3, 6, 12 or 24).
        /// </summary>
        /// <example>3</example>
        public int? interval { get; set; }

        /// <summary>
        /// Unit system: "m" for metric, "s" for scientific, "f" for Fahrenheit. Defaults to metric.
        /// </summary>
        /// <example>m</example>
        public string? units { get; set; }

        /// <summary>
        /// ISO language code used for the textual weather descriptions.
        /// </summary>
        /// <example>en</example>
        public string? language { get; set; }

        /// <summary>
        /// Optional JSONP callback name forwarded to the provider.
        /// </summary>
        public string? callback { get; set; }
    }
}
