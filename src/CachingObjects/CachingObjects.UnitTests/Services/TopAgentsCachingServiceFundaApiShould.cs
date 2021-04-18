using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Services;

    public class TopAgentsCachingServiceFundaApiShould : TopAgentsCachingServiceTestBase
    {
        public TopAgentsCachingServiceFundaApiShould()
        {
            SetupRepositoryDeleteAllStatingObject();
        }

        [Fact]
        public void ThrowExceptionWhenApiReturnsNull()
        {
            // arrange                        
            TopAgentsCachingService testing = GetServiceInstance(GetDefaultTopAgentsCachingOptions());

            FundaApiMock
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

            FundaApiMock
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

            var options = GetDefaultTopAgentsCachingOptions(pageSize);
            var testing = GetServiceInstance(options);
            SetupFundaApiGetObjects(totalPage, pageSize);

            // act
            await testing.ProsessCachingObjectsAsync();

            // assert
            FundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize), Times.Exactly(totalPage));
        }                
    }
}
