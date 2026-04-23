namespace ShowcaseProject.RestApi.Services
{
    /// <summary>
    /// Base service class for Showcase Project that provides common dependencies for derived services.
    /// </summary>
    public class ShowcaseProjectBaseService
    {
        /// <summary>
        /// Logger instance for logging service operations.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Configuration instance for accessing application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// HTTP client factory for creating HTTP client instances.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="IShowcaseProjectBaseService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="httpClientFactory">The HTTP client factory instance.</param>
        public ShowcaseProjectBaseService(ILogger logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
    }
}
