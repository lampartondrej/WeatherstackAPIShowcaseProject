using ShowcaseProject.Shared.Model.DTOs.Weatherstack.Current.Request;

namespace ShowcaseProject.Tests.DTOs.Weatherstack.Current.Request
{
    public class GetCurrentWeatherRequestTests
    {
        [Fact]
        public void GetCurrentWeatherRequest_WithRequiredProperties_IsValid()
        {
            // Arrange & Act
            var request = new GetCurrentWeatherRequest
            {
                Location = "Prague"
            };

            // Assert
            Assert.Equal("Prague", request.Location);
            Assert.Null(request.units);
            Assert.Null(request.language);
            Assert.Null(request.callback);
        }

        [Fact]
        public void GetCurrentWeatherRequest_WithAllProperties_SetsCorrectly()
        {
            // Arrange & Act
            var request = new GetCurrentWeatherRequest
            {
                Location = "London",
                units = "m",
                language = "en",
                callback = "weatherCallback"
            };

            // Assert
            Assert.Equal("London", request.Location);
            Assert.Equal("m", request.units);
            Assert.Equal("en", request.language);
            Assert.Equal("weatherCallback", request.callback);
        }

        [Fact]
        public void GetCurrentWeatherRequest_LocationProperty_CanBeSet()
        {
            // Arrange
            var request = new GetCurrentWeatherRequest
            {
                Location = "Paris"
            };

            // Act
            request.Location = "Berlin";

            // Assert
            Assert.Equal("Berlin", request.Location);
        }

        [Fact]
        public void GetCurrentWeatherRequest_OptionalProperties_CanBeNull()
        {
            // Arrange & Act
            var request = new GetCurrentWeatherRequest
            {
                Location = "Tokyo",
                units = null,
                language = null,
                callback = null
            };

            // Assert
            Assert.Equal("Tokyo", request.Location);
            Assert.Null(request.units);
            Assert.Null(request.language);
            Assert.Null(request.callback);
        }

        [Fact]
        public void GetCurrentWeatherRequest_WithSpecialCharacters_StoresCorrectly()
        {
            // Arrange & Act
            var request = new GetCurrentWeatherRequest
            {
                Location = "S?o Paulo"
            };

            // Assert
            Assert.Equal("S?o Paulo", request.Location);
        }

        [Theory]
        [InlineData("m")]
        [InlineData("f")]
        [InlineData("s")]
        public void GetCurrentWeatherRequest_WithDifferentUnits_SetsCorrectly(string units)
        {
            // Arrange & Act
            var request = new GetCurrentWeatherRequest
            {
                Location = "Test",
                units = units
            };

            // Assert
            Assert.Equal(units, request.units);
        }
    }
}
