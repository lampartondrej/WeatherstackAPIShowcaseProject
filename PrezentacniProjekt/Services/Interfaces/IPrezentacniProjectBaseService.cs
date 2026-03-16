namespace PrezentacniProjekt.Services.Interfaces
{
    public class IPrezentacniProjectBaseService
    {
        private readonly ILogger<IPrezentacniProjectBaseService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public IPrezentacniProjectBaseService(ILogger<IPrezentacniProjectBaseService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
    }
}
