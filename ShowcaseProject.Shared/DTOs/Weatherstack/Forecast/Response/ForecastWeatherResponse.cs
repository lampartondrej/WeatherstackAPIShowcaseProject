using System.Text.Json.Serialization;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Shared;

namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    public class ForecastWeatherResponse : IWeatherstackResponse
    {
        public ForecastWeatherResponseRequest? request { get; set; }
        public ForecastWeatherResponseLocation? location { get; set; }
        public ForecastWeatherResponseCurrent? current { get; set; }
        public ForecastWeatherResponseForecast? forecast { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WeatherstackError? error { get; set; }

        [JsonIgnore]
        public bool HasUsablePayload => location is not null;
    }
}
