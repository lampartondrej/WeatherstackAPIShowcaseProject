namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    public class ForecastWeatherResponseDay
    {
        public string? date { get; set; }
        public int? date_epoch { get; set; }
        public ForecastWeatherResponseAstro? astro { get; set; }
        public int? mintemp { get; set; }
        public int? maxtemp { get; set; }
        public int? avgtemp { get; set; }
        public int? totalsnow { get; set; }
        public float? sunhour { get; set; }
        public int? uv_index { get; set; }
        public ForecastWeatherResponseAir_Quality? air_quality { get; set; }
    }
}
