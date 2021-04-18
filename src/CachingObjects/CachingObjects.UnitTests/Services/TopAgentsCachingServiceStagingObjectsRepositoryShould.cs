using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Entities;
    using CachingObjectsWorkerService.Services;

    public class TopAgentsCachingServiceStagingObjectsRepositoryShould : TopAgentsCachingServiceTestBase
    {
        private readonly TopAgentsCachingService _testing;

        public TopAgentsCachingServiceStagingObjectsRepositoryShould()
        {
            _testing = GetServiceInstance(GetDefaultTopAgentsCachingOptions());
        }

        [Fact]
        public async Task ShouldCleanStagingObjectTable()
        {
            // arrange
            SetupFundaApiGetObjects();
            SetupRepositoryDeleteAllStatingObject(true);

            // act
            await _testing.ProsessCachingObjectsAsync();

            // assert
            FundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            StagingObjectRepositoryMock
                .Verify(repo => repo.DeleteAllStagingObjects(), Times.Once);
        }

        [Fact]
        public void ThrowExceptionWhenStagingTableCouldNotBeCleanedUp()
        {
            // arrange
            SetupFundaApiGetObjects();
            SetupRepositoryDeleteAllStatingObject(false);

            // act
            Func<Task> processFunc = async () => { await _testing.ProsessCachingObjectsAsync(); };

            // assert
            processFunc.Should().Throw<Exception>();
        }

        [Fact]
        public async Task ShouldSaveTheResponse()
        {
            // arrange
            var id = "id";
            var agentId = 1;
            var agentName = "agent";

            SetupFundaApiGetObjects(id: id, agentId: agentId, agentName: agentName);
            SetupRepositoryDeleteAllStatingObject(true);

            // act
            await _testing.ProsessCachingObjectsAsync();

            // assert
            FundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            StagingObjectRepositoryMock
                .Verify(repo => repo.CreateStagingObjects(
                    It.Is<ICollection<StagingObject>>(so =>
                        so.FirstOrDefault().Id == id &&
                        so.FirstOrDefault().AgentId == agentId.ToString() &&
                        so.FirstOrDefault().AgentName == agentName)
                    ),
                    Times.Once);
        }

        [Fact]
        public async Task ShouldGetTopAgentCount()
        {
            // arrange
            SetupFundaApiGetObjects();
            SetupRepositoryDeleteAllStatingObject(true);

            // act
            await _testing.ProsessCachingObjectsAsync();

            // assert
            StagingObjectRepositoryMock
                .Verify(repo => repo.GetTopAgentsByObjects(It.IsAny<int>()), Times.Once);
        }
    }
}
