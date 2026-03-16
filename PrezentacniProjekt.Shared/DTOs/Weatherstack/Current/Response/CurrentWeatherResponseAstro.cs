namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    public class CurrentWeatherResponseAstro
    {
        public string? sunrise { get; set; }
        public string? sunset { get; set; }
        public string? moonrise { get; set; }
        public string? moonset { get; set; }
        public string? moon_phase { get; set; }
        public int? moon_illumination { get; set; }
    }

}
