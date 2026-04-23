using ShowcaseProject.RestApi.CustomHelpers;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;
using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Request;

namespace ShowcaseProject.Tests.CustomHelpers
{
    public class BuildUriStringForWeatherstackTests
    {
        private readonly IWeatherstackRequestBuilder _uriBuilder;

        public BuildUriStringForWeatherstackTests()
        {
            _uriBuilder = new BuildUriStringForWeatherstack();
        }

        #region BuildQueryForCurrentWeather Tests

        [Fact]
        public void BuildQueryForCurrentWeather_WithLocationOnly_ReturnsLocationString()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "Prague"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("Prague", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithLocationAndUnits_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "London",
                units = "m"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("London&units=m", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithLocationAndLanguage_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "Paris",
                language = "fr"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("Paris&language=fr", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithLocationAndCallback_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "Berlin",
                callback = "myCallback"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("Berlin&callback=myCallback", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithAllParameters_ReturnsCompleteString()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "New York",
                units = "f",
                language = "en",
                callback = "testCallback"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("New%20York&units=f&language=en&callback=testCallback", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithEmptyOptionalParameters_IgnoresEmptyValues()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "Tokyo",
                units = "",
                language = "",
                callback = ""
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal("Tokyo", result);
        }

        [Fact]
        public void BuildQueryForCurrentWeather_WithSpecialCharactersInLocation_ReturnsLocation()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "S?o Paulo"
            };

            // Act
            var result = _uriBuilder.BuildQueryForCurrentWeather(request);

            // Assert
            Assert.Equal(Uri.EscapeDataString("S?o Paulo"), result);
        }

        #endregion

        #region BuildQueryForForecastWeather Tests

        [Fact]
        public void BuildQueryForForecastWeather_WithLocationOnly_ReturnsLocationString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Prague"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Prague", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithForecastDays_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "London",
                forecastDays = 5
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("London&forecast_days=5", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithHourly_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Paris",
                hourly = 1
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Paris&hourly=1", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithInterval_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Berlin",
                interval = 3
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Berlin&interval=3", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithUnits_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Rome",
                units = "m"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Rome&units=m", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithLanguage_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Madrid",
                language = "es"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Madrid&language=es", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithCallback_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Amsterdam",
                callback = "myCallback"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Amsterdam&callback=myCallback", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithAllParameters_ReturnsCompleteString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "New York",
                forecastDays = 7,
                hourly = 1,
                interval = 6,
                units = "f",
                language = "en",
                callback = "weatherCallback"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("New%20York&forecast_days=7&hourly=1&interval=6&units=f&language=en&callback=weatherCallback", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithEmptyOptionalParameters_IgnoresEmptyValues()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Tokyo",
                units = "",
                language = "",
                callback = ""
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Tokyo", result);
        }

        [Fact]
        public void BuildQueryForForecastWeather_WithMixedParameters_ReturnsCorrectString()
        {
            // Arrange
            var request = new GetForecastWeatherRequest
            {
                Location = "Vienna",
                forecastDays = 3,
                units = "m",
                language = "de"
            };

            // Act
            var result = _uriBuilder.BuildQueryForForecastWeather(request);

            // Assert
            Assert.Equal("Vienna&forecast_days=3&units=m&language=de", result);
        }

        #endregion
    }
}
