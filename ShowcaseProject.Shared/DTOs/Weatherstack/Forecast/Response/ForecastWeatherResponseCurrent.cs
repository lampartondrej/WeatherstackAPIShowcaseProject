namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    public class ForecastWeatherResponseCurrent
    {
        public string? observation_time { get; set; }
        public int? temperature { get; set; }
        public int? weather_code { get; set; }
        public string[]? weather_icons { get; set; }
        public string[]? weather_descriptions { get; set; }
        public ForecastWeatherResponseAir_Quality? air_quality { get; set; }
        public int? wind_speed { get; set; }
        public int? wind_degree { get; set; }
        public string? wind_dir { get; set; }
        public int? pressure { get; set; }
        public float? precip { get; set; }
        public int? humidity { get; set; }
        public int? cloudcover { get; set; }
        public int? feelslike { get; set; }
        public int? visibility { get; set; }
        public string? is_day { get; set; }
    }
}
