namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    /// <summary>
    /// Represents the location information from the Weatherstack API current weather response.
    /// </summary>
    public class CurrentWeatherResponseLocation
    {
        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// Gets or sets the country where the location is situated.
        /// </summary>
        public string? country { get; set; }

        /// <summary>
        /// Gets or sets the region where the location is situated.
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
        /// Gets or sets the timezone identifier for the location.
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
        /// Gets or sets the UTC offset for the location's timezone.
        /// </summary>
        public string? utc_offset { get; set; }
    }
}
