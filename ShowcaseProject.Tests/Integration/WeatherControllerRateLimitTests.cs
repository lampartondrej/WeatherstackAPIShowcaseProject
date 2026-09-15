using Moq;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.Wrapper;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace ShowcaseProject.Tests.Integration
{
    /// <summary>
    /// Groups the integration test classes into a single collection so they never run
    /// concurrently — both factories share the same process-wide environment variables.
    /// </summary>
    [CollectionDefinition(IntegrationTestCollection.Name)]
    public class IntegrationTestCollection
    {
        public const string Name = "Integration";
    }

    /// <summary>
    /// Host configured with a very small permit limit so the rate limiter can be
    /// exercised deterministically without waiting for a realistic window to fill.
    /// </summary>
    public class RateLimitedWebApplicationFactory : CustomWebApplicationFactory
    {
        public const int PermitLimit = 3;

        protected override int RateLimitPermitLimit => PermitLimit;
    }

    [Collection(IntegrationTestCollection.Name)]
    public class WeatherControllerRateLimitTests : IClassFixture<RateLimitedWebApplicationFactory>
    {
        private readonly RateLimitedWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public WeatherControllerRateLimitTests(RateLimitedWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes("testuser:testpass"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        [Fact]
        public async Task ExceedingPermitLimit_ReturnsTooManyRequestsWithRetryAfter()
        {
            // Arrange
            _factory.MockWeatherService!
                .Setup(x => x.GetCurrentWeather(It.IsAny<GetCurrentWeatherRequest>()))
                .ReturnsAsync(new ServiceWrapper<CurrentWeatherResponse>
                {
                    IsSuccess = true,
                    Data = new CurrentWeatherResponse
                    {
                        location = new CurrentWeatherResponseLocation { name = "Prague" },
                        current = new CurrentWeatherResponseCurrent { temperature = 20 }
                    }
                });

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act - consume the whole window
            for (var attempt = 1; attempt <= RateLimitedWebApplicationFactory.PermitLimit; attempt++)
            {
                var permitted = await _client.PostAsJsonAsync("/Weather/current", request);
                Assert.Equal(HttpStatusCode.OK, permitted.StatusCode);
            }

            var throttled = await _client.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.TooManyRequests, throttled.StatusCode);
            Assert.NotNull(throttled.Headers.RetryAfter);
        }
    }
}
