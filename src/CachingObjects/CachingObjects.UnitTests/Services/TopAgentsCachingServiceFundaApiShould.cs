using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;

    public class TopAgentsCachingServiceFundaApiShould : TopAgentsCachingServiceTestBase
    {
        private readonly Mock<IFundaApi> _fundaApiMock;
        private readonly Mock<ILogger<TopAgentsCachingService>> _loggerMock;

        public TopAgentsCachingServiceFundaApiShould()
        {
            _fundaApiMock = new Mock<IFundaApi>();
            _loggerMock = new Mock<ILogger<TopAgentsCachingService>>();
        }

        [Fact]
        public void ThrowExceptionWhenApiReturnsNull()
        {
            // arrange                        
            TopAgentsCachingService testing = GetServiceInstance(GetDefaultTopAgentsCachingOptions());

            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(null);

            // act
            Func<Task> func = async () => { await testing.ProsessCachingObjectsAsync(); };

            // assert
            func.Should().Throw<Exception>();
        }


        [Fact]
        public void ThrowExceptionWhenApiCallThrowsException()
        {
            // arrange            
            TopAgentsCachingService testing = GetServiceInstance(GetDefaultTopAgentsCachingOptions());

            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<Exception>();

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

            var options = GetDefaultTopAgentsCachingOptions(pageSize);
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

        private TopAgentsCachingService GetServiceInstance(IOptions<TopAgentsCachingOptions> options) =>
           new TopAgentsCachingService(_fundaApiMock.Object, options, _loggerMock.Object);
    }
}
