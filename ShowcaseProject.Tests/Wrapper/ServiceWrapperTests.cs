using ShowcaseProject.Shared.Model.Wrapper;

namespace ShowcaseProject.Tests.Wrapper
{
    public class ServiceWrapperTests
    {
        [Fact]
        public void ServiceWrapper_WithSuccessResponse_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var wrapper = new ServiceWrapper<string>
            {
                IsSuccess = true,
                Data = "Test Data",
                DetailedErrorMessage = null
            };

            // Assert
            Assert.True(wrapper.IsSuccess);
            Assert.Equal("Test Data", wrapper.Data);
            Assert.Null(wrapper.DetailedErrorMessage);
        }

        [Fact]
        public void ServiceWrapper_WithFailureResponse_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var wrapper = new ServiceWrapper<string>
            {
                IsSuccess = false,
                Data = null,
                DetailedErrorMessage = "Error occurred"
            };

            // Assert
            Assert.False(wrapper.IsSuccess);
            Assert.Null(wrapper.Data);
            Assert.Equal("Error occurred", wrapper.DetailedErrorMessage);
        }

        [Fact]
        public void ServiceWrapper_WithComplexType_HandlesDataCorrectly()
        {
            // Arrange
            var testObject = new TestClass { Id = 1, Name = "Test" };

            // Act
            var wrapper = new ServiceWrapper<TestClass>
            {
                IsSuccess = true,
                Data = testObject,
                DetailedErrorMessage = null
            };

            // Assert
            Assert.True(wrapper.IsSuccess);
            Assert.NotNull(wrapper.Data);
            Assert.Equal(1, wrapper.Data.Id);
            Assert.Equal("Test", wrapper.Data.Name);
            Assert.Null(wrapper.DetailedErrorMessage);
        }

        [Fact]
        public void ServiceWrapper_WithNullData_IsValid()
        {
            // Arrange & Act
            var wrapper = new ServiceWrapper<string>
            {
                IsSuccess = false,
                Data = null,
                DetailedErrorMessage = "Data not found"
            };

            // Assert
            Assert.False(wrapper.IsSuccess);
            Assert.Null(wrapper.Data);
            Assert.NotNull(wrapper.DetailedErrorMessage);
        }

        [Fact]
        public void ServiceWrapper_InitProperties_AreImmutable()
        {
            // Arrange
            var wrapper = new ServiceWrapper<string>
            {
                IsSuccess = true,
                Data = "Original Data",
                DetailedErrorMessage = null
            };

            // Assert - init properties cannot be reassigned after initialization
            Assert.True(wrapper.IsSuccess);
            Assert.Equal("Original Data", wrapper.Data);
            Assert.Null(wrapper.DetailedErrorMessage);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
