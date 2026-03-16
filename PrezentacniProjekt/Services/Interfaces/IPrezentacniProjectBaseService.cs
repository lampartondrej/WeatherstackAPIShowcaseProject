namespace PrezentacniProjekt.Services.Interfaces
{
    /// <summary>
    /// Base service class for Prezentacni Project that provides common dependencies for derived services.
    /// </summary>
    public class IPrezentacniProjectBaseService
    {
        /// <summary>
        /// Logger instance for logging service operations.
        /// </summary>
        private readonly ILogger<IPrezentacniProjectBaseService> _logger;

        /// <summary>
        /// Configuration instance for accessing application settings.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// HTTP client factory for creating HTTP client instances.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPrezentacniProjectBaseService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="httpClientFactory">The HTTP client factory instance.</param>
        public IPrezentacniProjectBaseService(ILogger<IPrezentacniProjectBaseService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
    }
}
