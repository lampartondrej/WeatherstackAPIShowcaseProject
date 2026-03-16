namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    public class CurrentWeatherResponseRequest
    {
        public string? type { get; set; }
        public string? query { get; set; }
        public string? language { get; set; }
        public string? unit { get; set; }
    }

}
