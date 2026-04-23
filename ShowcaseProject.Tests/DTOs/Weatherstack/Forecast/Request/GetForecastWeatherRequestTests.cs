using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;

namespace ShowcaseProject.Tests.DTOs.Weatherstack.Forecast.Request
{
    public class GetForecastWeatherRequestTests
    {
        [Fact]
        public void GetForecastWeatherRequest_WithRequiredProperties_IsValid()
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Prague"
            };

            // Assert
            Assert.Equal("Prague", request.Location);
            Assert.Null(request.forecastDays);
            Assert.Null(request.hourly);
            Assert.Null(request.interval);
            Assert.Null(request.units);
            Assert.Null(request.language);
            Assert.Null(request.callback);
        }

        [Fact]
        public void GetForecastWeatherRequest_WithAllProperties_SetsCorrectly()
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "London",
                forecastDays = 7,
                hourly = 1,
                interval = 3,
                units = "m",
                language = "en",
                callback = "forecastCallback"
            };

            // Assert
            Assert.Equal("London", request.Location);
            Assert.Equal(7, request.forecastDays);
            Assert.Equal(1, request.hourly);
            Assert.Equal(3, request.interval);
            Assert.Equal("m", request.units);
            Assert.Equal("en", request.language);
            Assert.Equal("forecastCallback", request.callback);
        }

        [Fact]
        public void GetForecastWeatherRequest_LocationProperty_CanBeSet()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Paris"
            };

            // Act
            request.Location = "Berlin";

            // Assert
            Assert.Equal("Berlin", request.Location);
        }

        [Fact]
        public void GetForecastWeatherRequest_OptionalProperties_CanBeNull()
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Tokyo",
                forecastDays = null,
                hourly = null,
                interval = null,
                units = null,
                language = null,
                callback = null
            };

            // Assert
            Assert.Equal("Tokyo", request.Location);
            Assert.Null(request.forecastDays);
            Assert.Null(request.hourly);
            Assert.Null(request.interval);
            Assert.Null(request.units);
            Assert.Null(request.language);
            Assert.Null(request.callback);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(14)]
        public void GetForecastWeatherRequest_WithDifferentForecastDays_SetsCorrectly(int days)
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Test",
                forecastDays = days
            };

            // Assert
            Assert.Equal(days, request.forecastDays);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetForecastWeatherRequest_WithDifferentHourlyValues_SetsCorrectly(int hourly)
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Test",
                hourly = hourly
            };

            // Assert
            Assert.Equal(hourly, request.hourly);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(12)]
        [InlineData(24)]
        public void GetForecastWeatherRequest_WithDifferentIntervals_SetsCorrectly(int interval)
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Test",
                interval = interval
            };

            // Assert
            Assert.Equal(interval, request.interval);
        }

        [Fact]
        public void GetForecastWeatherRequest_WithPartialOptionalProperties_SetsCorrectly()
        {
            // Arrange & Act
            var request = new GetForecastWeatherRequest
            {
                Location = "Vienna",
                forecastDays = 5,
                units = "m"
            };

            // Assert
            Assert.Equal("Vienna", request.Location);
            Assert.Equal(5, request.forecastDays);
            Assert.Null(request.hourly);
            Assert.Null(request.interval);
            Assert.Equal("m", request.units);
            Assert.Null(request.language);
            Assert.Null(request.callback);
        }
    }
}
