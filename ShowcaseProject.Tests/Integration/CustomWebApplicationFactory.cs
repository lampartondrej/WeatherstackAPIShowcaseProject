using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ShowcaseProject.Services.Interfaces;

namespace ShowcaseProject.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IWeatherService>? MockWeatherService { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Add test configuration
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"APIOptions:WeatherstackApiKeyEnvVar", "WeatherstackApiKey"},
                    {"APIOptions:WeatherstackApiUrl", "http://api.weatherstack.com"},
                    {"APIOptions:CurrentWeatherEndpoint", "current"},
                    {"APIOptions:ForecastWeatherEndpoint", "forecast"},
                    {"AuthSettings:UsernameEnvVar", "TestUsername"},
                    {"AuthSettings:PasswordEnvVar", "TestPassword"}
                }!);
            });

            builder.ConfigureServices(services =>
            {
                // Setup test environment variables
                Environment.SetEnvironmentVariable("WeatherstackApiKey", "test-api-key");
                Environment.SetEnvironmentVariable("TestUsername", "testuser");
                Environment.SetEnvironmentVariable("TestPassword", "testpass");

                // Remove the existing IWeatherService registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IWeatherService));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add mock IWeatherService
                MockWeatherService = new Mock<IWeatherService>();
                services.AddScoped<IWeatherService>(_ => MockWeatherService.Object);

                // Suppress logging in tests
                services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Critical);
                });
            });

            builder.UseEnvironment("Testing");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Environment.SetEnvironmentVariable("WeatherstackApiKey", null);
                Environment.SetEnvironmentVariable("TestUsername", null);
                Environment.SetEnvironmentVariable("TestPassword", null);
            }

            base.Dispose(disposing);
        }
    }
}
