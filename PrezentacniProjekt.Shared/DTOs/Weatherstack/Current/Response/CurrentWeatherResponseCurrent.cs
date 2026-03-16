namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    /// <summary>
    /// Represents the current weather data from the Weatherstack API response.
    /// </summary>
    public class CurrentWeatherResponseCurrent
    {
        /// <summary>
        /// Gets or sets the time when the weather observation was made.
        /// </summary>
        public string? observation_time { get; set; }

        /// <summary>
        /// Gets or sets the current temperature in the specified unit.
        /// </summary>
        public int? temperature { get; set; }

        /// <summary>
        /// Gets or sets the weather condition code.
        /// </summary>
        public int? weather_code { get; set; }

        /// <summary>
        /// Gets or sets the URLs of weather icons representing current conditions.
        /// </summary>
        public string[]? weather_icons { get; set; }

        /// <summary>
        /// Gets or sets the textual descriptions of current weather conditions.
        /// </summary>
        public string[]? weather_descriptions { get; set; }

        /// <summary>
        /// Gets or sets the astronomical data including sunrise, sunset, and moon information.
        /// </summary>
        public CurrentWeatherResponseAstro? astro { get; set; }

        /// <summary>
        /// Gets or sets the air quality measurements including pollutant levels and indices.
        /// </summary>
        public CurrentWeatherResponseAir_Quality? air_quality { get; set; }

        /// <summary>
        /// Gets or sets the wind speed in the specified unit.
        /// </summary>
        public int? wind_speed { get; set; }

        /// <summary>
        /// Gets or sets the wind direction in degrees.
        /// </summary>
        public int? wind_degree { get; set; }

        /// <summary>
        /// Gets or sets the wind direction as a compass point (e.g., "N", "NE", "E").
        /// </summary>
        public string? wind_dir { get; set; }

        /// <summary>
        /// Gets or sets the atmospheric pressure in millibars.
        /// </summary>
        public int? pressure { get; set; }

        /// <summary>
        /// Gets or sets the precipitation amount in the specified unit.
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
        /// Gets or sets the "feels like" temperature considering wind chill and humidity.
        /// </summary>
        public int? feelslike { get; set; }

        /// <summary>
        /// Gets or sets the UV index value.
        /// </summary>
        public int? uv_index { get; set; }

        /// <summary>
        /// Gets or sets the visibility distance in the specified unit.
        /// </summary>
        public int? visibility { get; set; }

        /// <summary>
        /// Gets or sets whether it is currently daytime ("yes") or nighttime ("no").
        /// </summary>
        public string? is_day { get; set; }
    }
}
