using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Response
{
    public class CurrentWeatherResponse
    {
        public CurrentWeatherResponseRequest? request { get; set; }
        public CurrentWeatherResponseLocation? location { get; set; }
        public CurrentWeatherResponseCurrent? current { get; set; }
    }
}
