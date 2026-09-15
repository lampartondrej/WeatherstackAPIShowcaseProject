using System.ComponentModel.DataAnnotations;

namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request
{
    /// <summary>
    /// Query for the current weather conditions at a single location.
    /// </summary>
    public class GetCurrentWeatherRequest
    {
        /// <summary>
        /// Location to look up: a city name, postal code, "latitude,longitude" pair or IP address.
        /// </summary>
        /// <example>Prague</example>
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public required string Location { get; set; }

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
