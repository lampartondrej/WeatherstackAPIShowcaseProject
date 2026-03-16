using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrezentacniProjekt.Controllers
{
    /// <summary>
    /// Base controller for the Prezentacni Projekt API.
    /// Provides common functionality and enforces authentication for all derived controllers.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Require authentication for all actions
    public class PrezentacniProjektBaseController : ControllerBase
    {
        /// <summary>
        /// Logger instance for logging operations within the controller.
        /// </summary>
        private readonly ILogger<PrezentacniProjektBaseController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrezentacniProjektBaseController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        public PrezentacniProjektBaseController(ILogger<PrezentacniProjektBaseController> logger)
        {
            _logger = logger;
        }
    }
}
