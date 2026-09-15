using System.Text.Json.Serialization;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Shared;

namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response
{
    public class CurrentWeatherResponse : IWeatherstackResponse
    {
        public CurrentWeatherResponseRequest? request { get; set; }
        public CurrentWeatherResponseLocation? location { get; set; }
        public CurrentWeatherResponseCurrent? current { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WeatherstackError? error { get; set; }

        [JsonIgnore]
        public bool HasUsablePayload => location is not null && current is not null;
    }
}
