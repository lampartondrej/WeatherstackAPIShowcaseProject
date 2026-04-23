using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShowcaseProject.Controllers
{
    /// <summary>
    /// Base controller for the Showcase Project API.
    /// Provides common functionality and enforces authentication for all derived controllers.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Require authentication for all actions
    public class ShowcaseProjectBaseController : ControllerBase
    {
        /// <summary>
        /// Logger instance for logging operations within the controller.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowcaseProjectBaseController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        public ShowcaseProjectBaseController(ILogger logger)
        {
            _logger = logger;
        }
    }
}
