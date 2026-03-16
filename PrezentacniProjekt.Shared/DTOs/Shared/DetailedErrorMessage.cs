using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrezentacniProjekt.Shared.Model.DTOs.Shared
{
    /// <summary>
    /// Represents a detailed error message containing error information and HTTP status code.
    /// </summary>
    public class DetailedErrorMessage
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Gets or sets the detailed description of the error.
        /// </summary>
        public required string Details { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code associated with the error.
        /// </summary>
        public required int HttpStatusCode { get; set; }
    }
}
