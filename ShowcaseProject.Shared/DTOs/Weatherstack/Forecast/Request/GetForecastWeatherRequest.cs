using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request
{
    public class GetForecastWeatherRequest
    {
        public required string Location { get; set; }
        public int? forecastDays { get; set; }
        public int? hourly { get; set; }
        public int? interval { get; set; }
        public string? units { get; set; }
        public string? language { get; set; }
        public string? callback { get; set; }
    }
}