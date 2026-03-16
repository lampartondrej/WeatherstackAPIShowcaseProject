namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    /// <summary>
    /// Represents the request parameters returned in the current weather response from Weatherstack API.
    /// </summary>
    public class CurrentWeatherResponseRequest
    {
        /// <summary>
        /// Gets or sets the type of the request (e.g., City, LatLon, IP, Zipcode).
        /// </summary>
        public string? type { get; set; }

        /// <summary>
        /// Gets or sets the query string used in the request.
        /// </summary>
        public string? query { get; set; }

        /// <summary>
        /// Gets or sets the language code used for the response.
        /// </summary>
        public string? language { get; set; }

        /// <summary>
        /// Gets or sets the unit system used (e.g., m for metric, f for Fahrenheit, s for scientific).
        /// </summary>
        public string? unit { get; set; }
    }
}
