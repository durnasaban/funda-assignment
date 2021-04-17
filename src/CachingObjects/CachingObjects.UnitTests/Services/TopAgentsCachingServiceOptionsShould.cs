using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;

    public class TopLocationBasedObjectsServiceLocationsShould : TopAgentsCachingServiceTestBase
    {
        private readonly Mock<IFundaApi> _fundaApiMock;
        private readonly Mock<ILogger<TopAgentsCachingService>> _loggerMock;

        public TopLocationBasedObjectsServiceLocationsShould()
        {
            _fundaApiMock = new Mock<IFundaApi>();
            _loggerMock = new Mock<ILogger<TopAgentsCachingService>>();

            SetupFundaApiGetLocationBasedObjects();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenEmptyTopLocationBasedObjectsOptions()
        {
            // arrange
            IOptions<TopAgentsCachingOptions> topLocationBasedObjectsOptions = null;

            // act
            Func<TopAgentsCachingService> serviceFunc = () => GetServiceInstance(topLocationBasedObjectsOptions);

            // assert
            serviceFunc.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenNullOrEmptyLocations()
        {
            // arrange & act
            Func<TopAgentsCachingService> serviceFuncWithNullLocations = () => GetServiceInstance(GetTopLocationBasedObjectOptions());
            Func<TopAgentsCachingService> serviceFuncWithEmptyLocations = () => GetServiceInstance(GetTopLocationBasedObjectOptions(1, Array.Empty<string>()));

            // assert
            serviceFuncWithNullLocations.Should().Throw<ArgumentNullException>();
            serviceFuncWithEmptyLocations.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("location1")]
        [InlineData("location1", "location2")]
        [InlineData("location1", "location2", "location3")]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenLocations(params string[] locations)
        {
            // arrange
            var options = GetTopLocationBasedObjectOptions(1, locations);

            var testing = GetServiceInstance(options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            foreach (var location in locations)
            {
                _fundaApiMock
                    .Verify(
                    api => api.GetObjects(location, It.IsAny<int>(), It.IsAny<int>()),
                    Times.AtLeastOnce,
                    $"Operation with \"{location}\" can not be verified");
            }
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenDefaultPageSize()
        {
            // arrange
            var options = GetTopLocationBasedObjectOptions(0, "location");

            // act
            Func<TopAgentsCachingService> service = () => GetServiceInstance(options);

            // assert
            service.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenPageSize()
        {
            // arrange
            var pageSize = 1;
            var options = GetTopLocationBasedObjectOptions(pageSize, "location");

            var testing = GetServiceInstance(options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            _fundaApiMock
                .Verify(
                api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize),
                Times.AtLeastOnce);
        }

        private void SetupFundaApiGetLocationBasedObjects()
        {
            var response = JObject.Parse(@"{'Paging': { 'AantalPaginas' : 1 }}");

            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(response);
        }

        private TopAgentsCachingService GetServiceInstance(IOptions<TopAgentsCachingOptions> options) =>
           new TopAgentsCachingService(_fundaApiMock.Object, options, _loggerMock.Object);
    }
}
