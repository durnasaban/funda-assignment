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
                new TopLocationBasedObjectsService(_fundaApiMock.Object, GetTopLocationBasedObjectOptions(Array.Empty<string>()));

            // assert
            serviceFuncWithNullLocations.Should().Throw<ArgumentNullException>();
            serviceFuncWithEmptyLocations.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenALocation()
        {
            // arrange

            var location = "default.location";
            var options = GetTopLocationBasedObjectOptions(location);

            var testing = new TopLocationBasedObjectsService(_fundaApiMock.Object, options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            _fundaApiMock.Verify(api => api.GetObjects(location, It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenLocations()
        {
            // arrange

            var location1 = "location1";
            var location2 = "location2";
            var options = GetTopLocationBasedObjectOptions(location1, location2);

            var testing = new TopLocationBasedObjectsService(_fundaApiMock.Object, options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            _fundaApiMock.Verify(api => api.GetObjects(location1, It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
            _fundaApiMock.Verify(api => api.GetObjects(location2, It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        private static IOptions<TopLocationBasedObjectsOptions> GetTopLocationBasedObjectOptions(params string[] locations)
        {
            var options = new TopLocationBasedObjectsOptions
            {
                Locations = locations
            };

            return Options.Create(options);
        }
    }
}
