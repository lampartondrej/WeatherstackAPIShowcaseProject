namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents the location information in a forecast weather response from Weatherstack API.
    /// </summary>
    public class ForecastWeatherResponseLocation
    {
        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// Gets or sets the country of the location.
        /// </summary>
        public string? country { get; set; }

        /// <summary>
        /// Gets or sets the region of the location.
        /// </summary>
        public string? region { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the location.
        /// </summary>
        public string? lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the location.
        /// </summary>
        public string? lon { get; set; }

        /// <summary>
        /// Gets or sets the timezone identifier of the location.
        /// </summary>
        public string? timezone_id { get; set; }

        /// <summary>
        /// Gets or sets the local time at the location.
        /// </summary>
        public string? localtime { get; set; }

        /// <summary>
        /// Gets or sets the local time in Unix epoch format.
        /// </summary>
        public int? localtime_epoch { get; set; }

        /// <summary>
        /// Gets or sets the UTC offset of the location's timezone.
        /// </summary>
        public string? utc_offset { get; set; }
    }
}
