using Moq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;

    public class TopLocationBasedObjectsServiceFundaApiShould : TopLocationBasedObjectsServiceTestBase
    {
        private readonly Mock<IFundaApi> _fundaApiMock;
        private readonly Mock<ILogger<TopLocationBasedObjectsService>> _loggerMock;

        public TopLocationBasedObjectsServiceFundaApiShould()
        {
            _fundaApiMock = new Mock<IFundaApi>();
            _loggerMock = new Mock<ILogger<TopLocationBasedObjectsService>>();
        }

        [Fact]
        public void ThrowExceptionWhenApiReturnsNull()
        {
            // arrange            
            var options = GetTopLocationBasedObjectOptions(1, "location");
            TopLocationBasedObjectsService testing = GetServiceInstance(options);

            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null);

            // act
            Func<Task> func = async () => { await testing.ProsessCachingObjectsAsync(); };

            // assert
            func.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        public async Task ShouldCallFundaApiNTimesWhenApiReturnsTotalPageIsN(int totalPage)
        {
            // arrange
            var pageSize = 1;
            var response = JObject.Parse(@"{'Paging': { 'AantalPaginas' : " + totalPage + " }}");

            var options = GetTopLocationBasedObjectOptions(pageSize, "location");
            var testing = GetServiceInstance(options);

            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize))
                .ReturnsAsync(response);


            // act
            await testing.ProsessCachingObjectsAsync();

            // assert
            _fundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize), Times.Exactly(totalPage));
        }

        private TopLocationBasedObjectsService GetServiceInstance(IOptions<TopLocationBasedObjectsOptions> options) =>
           new TopLocationBasedObjectsService(_fundaApiMock.Object, options, _loggerMock.Object);
    }
}
