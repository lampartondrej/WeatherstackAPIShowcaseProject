namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents the request information returned in the forecast weather response from Weatherstack API.
    /// </summary>
    public class ForecastWeatherResponseRequest
    {
        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        public string? type { get; set; }

        /// <summary>
        /// Gets or sets the query string used in the request.
        /// </summary>
        public string? query { get; set; }

        /// <summary>
        /// Gets or sets the language code used in the request.
        /// </summary>
        public string? language { get; set; }

        /// <summary>
        /// Gets or sets the unit system used in the request (e.g., metric, imperial).
        /// </summary>
        public string? unit { get; set; }
    }
}
