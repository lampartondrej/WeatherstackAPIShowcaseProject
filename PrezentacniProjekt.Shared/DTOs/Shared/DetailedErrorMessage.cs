using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Shared
{
    public class DetailedErrorMessage
    {
        public required string Message { get; set; }
        public required string Details { get; set; }
        public required int HttpStatusCode { get; set; }
    }
}
