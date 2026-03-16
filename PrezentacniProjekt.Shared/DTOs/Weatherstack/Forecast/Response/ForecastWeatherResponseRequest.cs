namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    public class ForecastWeatherResponseRequest
    {
        public string? type { get; set; }
        public string? query { get; set; }
        public string? language { get; set; }
        public string? unit { get; set; }
    }
}
