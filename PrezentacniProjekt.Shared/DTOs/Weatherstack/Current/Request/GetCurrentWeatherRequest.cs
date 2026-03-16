using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Weatherstack.Current.Request
{
    public class GetCurrentWeatherRequest
    {
        public required string Location { get; set; }
        public string? units { get; set; }
        public string? language { get; set; }
        public string? callback { get; set; }
    }
}
