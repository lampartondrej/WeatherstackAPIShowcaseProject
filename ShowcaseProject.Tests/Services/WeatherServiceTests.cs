using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using ShowcaseProject.Services;
using ShowcaseProject.Services.Interfaces;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Response;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response;
using ShowcaseProject.Shared.Model.Wrapper;
using System.Net;
using System.Text.Json;

namespace ShowcaseProject.Tests.Services
{
    public class WeatherServiceTests : IDisposable
    {
        private readonly Mock<ILogger<IShowcaseProjectBaseService>> _mockLogger;
        private readonly IConfiguration _configuration;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private const string TestApiKey = "test-api-key";
        private const string TestApiUrl = "http://api.weatherstack.com";

        public WeatherServiceTests()
        {
            _mockLogger = new Mock<ILogger<IShowcaseProjectBaseService>>();
            _configuration = BuildConfiguration();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _mockMemoryCache = new Mock<IMemoryCache>();

            // Setup memory cache to return false for TryGetValue (cache miss) by default
            object? cachedValue = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
                .Returns(false);

            // Setup environment variable
            Environment.SetEnvironmentVariable("WeatherstackApiKey", TestApiKey);
        }

        private IConfiguration BuildConfiguration()
        {
            var configData = new Dictionary<string, string>
            {
                {"APIOptions:WeatherstackApiKeyEnvVar", "WeatherstackApiKey"},
                {"APIOptions:WeatherstackApiUrl", TestApiUrl},
                {"APIOptions:CurrentWeatherEndpoint", "current"},
                {"APIOptions:ForecastWeatherEndpoint", "forecast"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();
        }

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string responseContent)
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });

            return new HttpClient(_mockHttpMessageHandler.Object);
        }

        #region GetCurrentWeather Tests

        [Fact]
        public async Task GetCurrentWeather_WithValidResponse_ReturnsSuccessWrapper()
        {
            // Arrange
            var expectedResponse = new CurrentWeatherResponse
            {
                location = new CurrentWeatherResponseLocation { name = "Prague" },
                current = new CurrentWeatherResponseCurrent { temperature = 20 }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var mockCacheEntry = new Mock<ICacheEntry>();
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>());
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Null(result.DetailedErrorMessage);
            Assert.Equal("Prague", result.Data.location?.name);
        }

        [Fact]
        public async Task GetCurrentWeather_WithBadRequest_ReturnsFailureWrapper()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, "");

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest { Location = "InvalidCity" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
            Assert.Contains("BadRequest", result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetCurrentWeather_WithServerError_ReturnsFailureWrapper()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
            Assert.Contains("InternalServerError", result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetCurrentWeather_WithHttpException_ReturnsFailureWrapper()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
            Assert.Contains("error occurred", result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetCurrentWeather_WithComplexRequest_BuildsCorrectUri()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}")
                });

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest
            {
                Location = "Prague",
                units = "m",
                language = "en"
            };

            // Act
            await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(capturedRequest);
            var uri = capturedRequest.RequestUri?.ToString();
            Assert.Contains("Prague", uri);
            Assert.Contains("units=m", uri);
            Assert.Contains("language=en", uri);
            Assert.Contains("access_key=" + TestApiKey, uri);
        }

        [Fact]
        public async Task GetCurrentWeather_LogsInformation_OnSuccess()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new CurrentWeatherResponse());
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            await service.GetCurrentWeather(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Requested current weather data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        #endregion

        #region GetForecastWeather Tests

        [Fact]
        public async Task GetForecastWeather_WithValidResponse_ReturnsSuccessWrapper()
        {
            // Arrange
            var expectedResponse = new ForecastWeatherResponse
            {
                location = new ForecastWeatherResponseLocation { name = "London" }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var mockCacheEntry = new Mock<ICacheEntry>();
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>());
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest { Location = "London" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Null(result.DetailedErrorMessage);
            Assert.Equal("London", result.Data.location?.name);
        }

        [Fact]
        public async Task GetForecastWeather_WithBadRequest_ReturnsFailureWrapper()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, "");

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest { Location = "InvalidCity" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
            Assert.Contains("BadRequest", result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetForecastWeather_WithUnauthorized_ReturnsFailureWrapper()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.Unauthorized, "");

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest { Location = "Paris" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
            Assert.Contains("Unauthorized", result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetForecastWeather_WithComplexRequest_BuildsCorrectUri()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}")
                });

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest
            {
                Location = "Berlin",
                forecastDays = 7,
                hourly = 1,
                units = "m"
            };

            // Act
            await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(capturedRequest);
            var uri = capturedRequest.RequestUri?.ToString();
            Assert.Contains("Berlin", uri);
            Assert.Contains("forecast_days=7", uri);
            Assert.Contains("hourly=1", uri);
            Assert.Contains("units=m", uri);
            Assert.Contains("access_key=" + TestApiKey, uri);
        }

        [Fact]
        public async Task GetForecastWeather_WithHttpException_ReturnsFailureWrapper()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Connection timeout"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest { Location = "Tokyo" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.DetailedErrorMessage);
        }

        [Fact]
        public async Task GetForecastWeather_LogsError_OnException()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Test exception"));

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);

            var request = new GetForecastWeatherRequest { Location = "Madrid" };

            // Act
            await service.GetForecastWeather(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region Configuration Tests

        [Fact]
        public void WeatherService_WithMissingApiKeyEnvVar_ThrowsException()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()!)
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                new WeatherService(_mockLogger.Object, emptyConfig, _mockHttpClientFactory.Object, _mockMemoryCache.Object));
        }

        [Fact]
        public void WeatherService_WithMissingApiUrl_ThrowsException()
        {
            // Arrange
            var partialConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"APIOptions:WeatherstackApiKeyEnvVar", "WeatherstackApiKey"}
                }!)
                .Build();

            Environment.SetEnvironmentVariable("WeatherstackApiKey", "test-key");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                new WeatherService(_mockLogger.Object, partialConfig, _mockHttpClientFactory.Object, _mockMemoryCache.Object));
        }

        #endregion

        #region Caching Tests

        [Fact]
        public async Task GetCurrentWeather_WithCachedData_ReturnsCachedResponse()
        {
            // Arrange
            var expectedResponse = new CurrentWeatherResponse
            {
                location = new CurrentWeatherResponseLocation { name = "Prague" },
                current = new CurrentWeatherResponseCurrent { temperature = 25 }
            };

            var cachedWrapper = new ServiceWrapper<CurrentWeatherResponse>
            {
                IsSuccess = true,
                Data = expectedResponse,
                DetailedErrorMessage = null
            };

            object? cacheEntry = cachedWrapper;
            _mockMemoryCache
                .Setup(x => x.TryGetValue("current_weather_Prague", out cacheEntry))
                .Returns(true);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetCurrentWeatherRequest { Location = "Prague" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(25, result.Data?.current?.temperature);

            // Verify HTTP client was never called
            _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);

            // Verify cache hit was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Returning cached current weather data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeather_WithoutCachedData_CallsApiAndCachesResult()
        {
            // Arrange
            var expectedResponse = new CurrentWeatherResponse
            {
                location = new CurrentWeatherResponseLocation { name = "Berlin" },
                current = new CurrentWeatherResponseCurrent { temperature = 18 }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            object? cacheEntry = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry))
                .Returns(false);

            var mockCacheEntry = new Mock<ICacheEntry>();
            object? cachedValue = null;
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>())
                .Callback<object>(v => cachedValue = v);
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetCurrentWeatherRequest { Location = "Berlin" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Berlin", result.Data?.location?.name);

            // Verify cache entry was created
            _mockMemoryCache.Verify(
                x => x.CreateEntry(It.IsAny<object>()),
                Times.Once);

            // Verify caching was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cached current weather data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetForecastWeather_WithCachedData_ReturnsCachedResponse()
        {
            // Arrange
            var expectedResponse = new ForecastWeatherResponse
            {
                location = new ForecastWeatherResponseLocation { name = "London" }
            };

            var cachedWrapper = new ServiceWrapper<ForecastWeatherResponse>
            {
                IsSuccess = true,
                Data = expectedResponse,
                DetailedErrorMessage = null
            };

            object? cacheEntry = cachedWrapper;
            _mockMemoryCache
                .Setup(x => x.TryGetValue("forecast_weather_London", out cacheEntry))
                .Returns(true);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetForecastWeatherRequest { Location = "London" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("London", result.Data?.location?.name);

            // Verify HTTP client was never called
            _mockHttpClientFactory.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);

            // Verify cache hit was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Returning cached forecast weather data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetForecastWeather_WithoutCachedData_CallsApiAndCachesResult()
        {
            // Arrange
            var expectedResponse = new ForecastWeatherResponse
            {
                location = new ForecastWeatherResponseLocation { name = "Paris" }
            };

            var jsonResponse = JsonSerializer.Serialize(expectedResponse);
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            object? cacheEntry = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry))
                .Returns(false);

            var mockCacheEntry = new Mock<ICacheEntry>();
            object? cachedValue = null;
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>())
                .Callback<object>(v => cachedValue = v);
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetForecastWeatherRequest { Location = "Paris" };

            // Act
            var result = await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Paris", result.Data?.location?.name);

            // Verify cache entry was created
            _mockMemoryCache.Verify(
                x => x.CreateEntry(It.IsAny<object>()),
                Times.Once);

            // Verify caching was logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cached forecast weather data")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeather_OnApiFailure_DoesNotCacheResult()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "");

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            object? cacheEntry = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry))
                .Returns(false);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetCurrentWeatherRequest { Location = "InvalidCity" };

            // Act
            var result = await service.GetCurrentWeather(request);

            // Assert
            Assert.False(result.IsSuccess);

            // Verify cache entry was never created
            _mockMemoryCache.Verify(
                x => x.CreateEntry(It.IsAny<object>()),
                Times.Never);
        }

        [Fact]
        public async Task GetCurrentWeather_CacheKey_IncludesLocation()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new CurrentWeatherResponse());
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            string? capturedCacheKey = null;
            object? cacheEntry = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry))
                .Callback(new TryGetValueCallback((object key, out object? value) =>
                {
                    capturedCacheKey = key.ToString();
                    value = null;
                }))
                .Returns(false);

            var mockCacheEntry = new Mock<ICacheEntry>();
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>());
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetCurrentWeatherRequest { Location = "Tokyo" };

            // Act
            await service.GetCurrentWeather(request);

            // Assert
            Assert.NotNull(capturedCacheKey);
            Assert.Equal("current_weather_Tokyo", capturedCacheKey);
        }

        private delegate void TryGetValueCallback(object key, out object? value);

        [Fact]
        public async Task GetForecastWeather_CacheKey_IncludesLocation()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new ForecastWeatherResponse());
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            string? capturedCacheKey = null;
            object? cacheEntry = null;
            _mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out cacheEntry))
                .Callback(new TryGetValueCallback((object key, out object? value) =>
                {
                    capturedCacheKey = key.ToString();
                    value = null;
                }))
                .Returns(false);

            var mockCacheEntry = new Mock<ICacheEntry>();
            mockCacheEntry.SetupSet(x => x.Value = It.IsAny<object>());
            mockCacheEntry.SetupSet(x => x.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>());

            _mockMemoryCache
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);

            var service = new WeatherService(_mockLogger.Object, _configuration, _mockHttpClientFactory.Object, _mockMemoryCache.Object);
            var request = new GetForecastWeatherRequest { Location = "Sydney" };

            // Act
            await service.GetForecastWeather(request);

            // Assert
            Assert.NotNull(capturedCacheKey);
            Assert.Equal("forecast_weather_Sydney", capturedCacheKey);
        }

        #endregion

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("WeatherstackApiKey", null);
            _mockHttpMessageHandler.Object.Dispose();
        }
    }
}
