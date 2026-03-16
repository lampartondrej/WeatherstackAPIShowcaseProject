namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents current weather data from the Weatherstack forecast response.
    /// </summary>
    public class ForecastWeatherResponseCurrent
    {
        /// <summary>
        /// Gets or sets the observation time.
        /// </summary>
        public string? observation_time { get; set; }

        /// <summary>
        /// Gets or sets the temperature in degrees.
        /// </summary>
        public int? temperature { get; set; }

        /// <summary>
        /// Gets or sets the weather code.
        /// </summary>
        public int? weather_code { get; set; }

        /// <summary>
        /// Gets or sets the weather icon URLs.
        /// </summary>
        public string[]? weather_icons { get; set; }

        /// <summary>
        /// Gets or sets the weather descriptions.
        /// </summary>
        public string[]? weather_descriptions { get; set; }

        /// <summary>
        /// Gets or sets the air quality data.
        /// </summary>
        public ForecastWeatherResponseAir_Quality? air_quality { get; set; }

        /// <summary>
        /// Gets or sets the wind speed.
        /// </summary>
        public int? wind_speed { get; set; }

        /// <summary>
        /// Gets or sets the wind direction in degrees.
        /// </summary>
        public int? wind_degree { get; set; }

        /// <summary>
        /// Gets or sets the wind direction abbreviation.
        /// </summary>
        public string? wind_dir { get; set; }

        /// <summary>
        /// Gets or sets the atmospheric pressure.
        /// </summary>
        public int? pressure { get; set; }

        /// <summary>
        /// Gets or sets the precipitation amount.
        /// </summary>
        public float? precip { get; set; }

        /// <summary>
        /// Gets or sets the humidity percentage.
        /// </summary>
        public int? humidity { get; set; }

        /// <summary>
        /// Gets or sets the cloud cover percentage.
        /// </summary>
        public int? cloudcover { get; set; }

        /// <summary>
        /// Gets or sets the "feels like" temperature.
        /// </summary>
        public int? feelslike { get; set; }

        /// <summary>
        /// Gets or sets the UV index.
        /// </summary>
        public int? uv_index { get; set; }

        /// <summary>
        /// Gets or sets the visibility distance.
        /// </summary>
        public int? visibility { get; set; }

        /// <summary>
        /// Gets or sets whether it is currently day time.
        /// </summary>
        public string? is_day { get; set; }
    }
}
