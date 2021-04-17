using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;
    using Moq;

    public class TopLocationBasedObjectsServiceLocationsShould
    {
        private readonly Mock<IFundaApi> _fundaApiMock;

        public TopLocationBasedObjectsServiceLocationsShould()
        {
            _fundaApiMock = new Mock<IFundaApi>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenEmptyTopLocationBasedObjectsOptions()
        {
            // arrange
            IOptions<TopLocationBasedObjectsOptions> topLocationBasedObjectsOptions = null;

            // act
            Func<TopLocationBasedObjectsService> serviceFunc = () => new TopLocationBasedObjectsService(_fundaApiMock.Object, topLocationBasedObjectsOptions);

            // assert
            serviceFunc.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenNullOrEmptyLocations()
        {
            // arrange & act
            Func<TopLocationBasedObjectsService> serviceFuncWithNullLocations = () =>
                new TopLocationBasedObjectsService(_fundaApiMock.Object, GetTopLocationBasedObjectOptions());

            Func<TopLocationBasedObjectsService> serviceFuncWithEmptyLocations = () =>
                new TopLocationBasedObjectsService(_fundaApiMock.Object, GetTopLocationBasedObjectOptions(1, Array.Empty<string>()));

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

            var testing = new TopLocationBasedObjectsService(_fundaApiMock.Object, options);

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
            Func<TopLocationBasedObjectsService> service = () => new TopLocationBasedObjectsService(_fundaApiMock.Object, options);

            // assert
            service.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenPageSize()
        {
            // arrange
            var pageSize = 1;
            var options = GetTopLocationBasedObjectOptions(pageSize, "location");

            var testing = new TopLocationBasedObjectsService(_fundaApiMock.Object, options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            _fundaApiMock
                .Verify(
                api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize),
                Times.AtLeastOnce);
            
        }

        private static IOptions<TopLocationBasedObjectsOptions> GetTopLocationBasedObjectOptions(int pageSize = 1, params string[] locations)
        {
            var options = new TopLocationBasedObjectsOptions
            {
                Locations = locations,
                PageSize = pageSize
            };

            return Options.Create(options);
        }
    }
}
