namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    public class CurrentWeatherResponseLocation
    {
        public string? name { get; set; }
        public string? country { get; set; }
        public string? region { get; set; }
        public string? lat { get; set; }
        public string? lon { get; set; }
        public string? timezone_id { get; set; }
        public string? localtime { get; set; }
        public int? localtime_epoch { get; set; }
        public string? utc_offset { get; set; }
    }

}
