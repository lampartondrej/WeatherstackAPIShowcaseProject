using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    public class ForecastWeatherResponse
    {
        public ForecastWeatherResponseRequest? request { get; set; }
        public ForecastWeatherResponseLocation? location { get; set; }
        public ForecastWeatherResponseCurrent? current { get; set; }
        public ForecastWeatherResponseForecast? forecast { get; set; }
    }
}
