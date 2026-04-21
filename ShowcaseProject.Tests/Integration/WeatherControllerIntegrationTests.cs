using Moq;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.Wrapper;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ShowcaseProject.Tests.Integration
{
    public class WeatherControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public WeatherControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            // Setup basic authentication
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("testuser:testpass"));
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
        }

        #region Current Weather Endpoint Tests

        [Fact]
        public async Task GetCurrentWeather_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var expectedResponse = new CurrentWeatherResponse
            {
                location = new CurrentWeatherResponseLocation { name = "Prague" },
                current = new CurrentWeatherResponseCurrent { temperature = 20 }
            };

            _factory.MockWeatherService!
                .Setup(x => x.GetCurrentWeather(It.IsAny<GetCurrentWeatherRequest>()))
                .ReturnsAsync(new ServiceWrapper<CurrentWeatherResponse>
                {
                    IsSuccess = true,
                    Data = expectedResponse,
                    DetailedErrorMessage = null
                });

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var weatherResponse = JsonSerializer.Deserialize<CurrentWeatherResponse>(content, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(weatherResponse);
            Assert.Equal("Prague", weatherResponse.location?.name);
        }

        [Fact]
        public async Task GetCurrentWeather_WithInvalidLocation_ReturnsBadRequest()
        {
            // Arrange
            _factory.MockWeatherService!
                .Setup(x => x.GetCurrentWeather(It.IsAny<GetCurrentWeatherRequest>()))
                .ReturnsAsync(new ServiceWrapper<CurrentWeatherResponse>
                {
                    IsSuccess = false,
                    Data = null,
                    DetailedErrorMessage = "Invalid location"
                });

            var request = new GetCurrentWeatherRequest { Location = "InvalidCity" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentWeather_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _factory.MockWeatherService!
                .Setup(x => x.GetCurrentWeather(It.IsAny<GetCurrentWeatherRequest>()))
                .ThrowsAsync(new Exception("Service error"));

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetCurrentWeather_WithAllParameters_PassesToService()
        {
            // Arrange
            GetCurrentWeatherRequest? capturedRequest = null;

            _factory.MockWeatherService!
                .Setup(x => x.GetCurrentWeather(It.IsAny<GetCurrentWeatherRequest>()))
                .Callback<GetCurrentWeatherRequest>(req => capturedRequest = req)
                .ReturnsAsync(new ServiceWrapper<CurrentWeatherResponse>
                {
                    IsSuccess = true,
                    Data = new CurrentWeatherResponse(),
                    DetailedErrorMessage = null
                });

            var request = new GetCurrentWeatherRequest
            {
                Location = "London",
                units = "m",
                language = "en",
                callback = "test"
            };

            // Act
            await _client.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("London", capturedRequest.Location);
            Assert.Equal("m", capturedRequest.units);
            Assert.Equal("en", capturedRequest.language);
            Assert.Equal("test", capturedRequest.callback);
        }

        [Fact]
        public async Task GetCurrentWeather_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithoutAuth = _factory.CreateClient();
            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var response = await clientWithoutAuth.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Forecast Weather Endpoint Tests

        [Fact]
        public async Task GetForecastWeather_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var expectedResponse = new ForecastWeatherResponse
            {
                location = new ForecastWeatherResponseLocation { name = "Berlin" }
            };

            _factory.MockWeatherService!
                .Setup(x => x.GetForecastWeather(It.IsAny<GetForecastWeatherRequest>()))
                .ReturnsAsync(new ServiceWrapper<ForecastWeatherResponse>
                {
                    IsSuccess = true,
                    Data = expectedResponse,
                    DetailedErrorMessage = null
                });

            var request = new GetForecastWeatherRequest { Location = "Berlin" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/forecast", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var forecastResponse = JsonSerializer.Deserialize<ForecastWeatherResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(forecastResponse);
            Assert.Equal("Berlin", forecastResponse.location?.name);
        }

        [Fact]
        public async Task GetForecastWeather_WithServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            _factory.MockWeatherService!
                .Setup(x => x.GetForecastWeather(It.IsAny<GetForecastWeatherRequest>()))
                .ReturnsAsync(new ServiceWrapper<ForecastWeatherResponse>
                {
                    IsSuccess = false,
                    Data = null,
                    DetailedErrorMessage = "API error"
                });

            var request = new GetForecastWeatherRequest { Location = "Paris" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/forecast", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetForecastWeather_WithComplexRequest_PassesAllParametersToService()
        {
            // Arrange
            GetForecastWeatherRequest? capturedRequest = null;

            _factory.MockWeatherService!
                .Setup(x => x.GetForecastWeather(It.IsAny<GetForecastWeatherRequest>()))
                .Callback<GetForecastWeatherRequest>(req => capturedRequest = req)
                .ReturnsAsync(new ServiceWrapper<ForecastWeatherResponse>
                {
                    IsSuccess = true,
                    Data = new ForecastWeatherResponse(),
                    DetailedErrorMessage = null
                });

            var request = new GetForecastWeatherRequest
            {
                Location = "Tokyo",
                forecastDays = 7,
                hourly = 1,
                interval = 3,
                units = "m",
                language = "en"
            };

            // Act
            await _client.PostAsJsonAsync("/Weather/forecast", request);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal("Tokyo", capturedRequest.Location);
            Assert.Equal(7, capturedRequest.forecastDays);
            Assert.Equal(1, capturedRequest.hourly);
            Assert.Equal(3, capturedRequest.interval);
            Assert.Equal("m", capturedRequest.units);
            Assert.Equal("en", capturedRequest.language);
        }

        [Fact]
        public async Task GetForecastWeather_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _factory.MockWeatherService!
                .Setup(x => x.GetForecastWeather(It.IsAny<GetForecastWeatherRequest>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            var request = new GetForecastWeatherRequest { Location = "Madrid" };

            // Act
            var response = await _client.PostAsJsonAsync("/Weather/forecast", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region Health Check Tests

        [Fact]
        public async Task HealthCheck_ReturnsHealthy()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Healthy", content);
        }

        #endregion

        #region Authentication Tests

        [Fact]
        public async Task GetCurrentWeather_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithInvalidAuth = _factory.CreateClient();
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("wronguser:wrongpass"));
            clientWithInvalidAuth.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var response = await clientWithInvalidAuth.PostAsJsonAsync("/Weather/current", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion
    }
}
