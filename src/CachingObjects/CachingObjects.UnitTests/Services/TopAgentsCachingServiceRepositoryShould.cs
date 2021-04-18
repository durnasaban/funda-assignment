using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Entities;
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Repositories;
    using CachingObjectsWorkerService.Services;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;

    public class TopAgentsCachingServiceRepositoryShould : TopAgentsCachingServiceTestBase
    {
        private readonly TopAgentsCachingService _testing;

        private readonly Mock<IFundaApi> _fundaApiMock;
        private readonly Mock<ILogger<TopAgentsCachingService>> _loggerMock;
        private readonly Mock<IStagingObjectRepository> _repositoryMock;

        public TopAgentsCachingServiceRepositoryShould()
        {
            _fundaApiMock = new Mock<IFundaApi>();
            _loggerMock = new Mock<ILogger<TopAgentsCachingService>>();
            _repositoryMock = new Mock<IStagingObjectRepository>();

            _testing = new TopAgentsCachingService(
                _fundaApiMock.Object,
                GetDefaultTopAgentsCachingOptions(),
                _loggerMock.Object,
                _repositoryMock.Object);
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
            _fundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            _repositoryMock
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

            SetupFundaApiGetObjects(id, agentId, agentName);
            SetupRepositoryDeleteAllStatingObject(true);

            // act
            await _testing.ProsessCachingObjectsAsync();

            // assert
            _fundaApiMock
                .Verify(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            _repositoryMock
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
            _repositoryMock
                .Verify(repo => repo.GetTopAgentsByObjects(It.IsAny<int>()), Times.Once);
        }


        private void SetupRepositoryDeleteAllStatingObject(bool result) =>
            _repositoryMock
                .Setup(repo => repo.DeleteAllStagingObjects())
                .ReturnsAsync(result);

        private void SetupFundaApiGetObjects(string id = "id", int agentId = 1, string agentName = "agent")
        {
            var response = JObject.Parse(@"{'Paging': { 'AantalPaginas' : '1' }, 'Objects':[{'Id':'" + id + "', 'MakelaarId': " + agentId + ", 'MakelaarNaam': '" + agentName + "'}]}");
            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), 1))
                .ReturnsAsync(response);
        }
    }
}
