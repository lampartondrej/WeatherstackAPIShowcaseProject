namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents air quality data from the Weatherstack forecast response.
    /// </summary>
    public class ForecastWeatherResponseAir_Quality
    {
        /// <summary>
        /// Gets or sets the carbon monoxide level.
        /// </summary>
        public string? co { get; set; }

        /// <summary>
        /// Gets or sets the nitrogen dioxide level.
        /// </summary>
        public string? no2 { get; set; }

        /// <summary>
        /// Gets or sets the ozone level.
        /// </summary>
        public string? o3 { get; set; }

        /// <summary>
        /// Gets or sets the sulfur dioxide level.
        /// </summary>
        public string? so2 { get; set; }

        /// <summary>
        /// Gets or sets the particulate matter 2.5 level.
        /// </summary>
        public string? pm2_5 { get; set; }

        /// <summary>
        /// Gets or sets the particulate matter 10 level.
        /// </summary>
        public string? pm10 { get; set; }

        /// <summary>
        /// Gets or sets the US EPA air quality index.
        /// </summary>
        public string? usepaindex { get; set; }

        /// <summary>
        /// Gets or sets the UK DEFRA air quality index.
        /// </summary>
        public string? gbdefraindex { get; set; }
    }
}
