using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShowcaseProject.Controllers
{
    /// <summary>
    /// Base controller for the Showcase Project API.
    /// Applies the shared routing convention and enforces authentication for all derived controllers.
    /// Abstract so it is not itself discovered as a routable controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize] // Require authentication for all actions
    public abstract class ShowcaseProjectBaseController : ControllerBase
    {
    }
}
