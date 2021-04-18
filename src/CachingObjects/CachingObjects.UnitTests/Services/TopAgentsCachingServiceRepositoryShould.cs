using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Repositories;
    using CachingObjectsWorkerService.Services;


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

        private void SetupRepositoryDeleteAllStatingObject(bool result) =>
            _repositoryMock
                .Setup(repo => repo.DeleteAllStagingObjects())
                .ReturnsAsync(result);

        private void SetupFundaApiGetObjects()
        {
            var response = JObject.Parse(@"{'Paging': { 'AantalPaginas' : '1' }, 'Id':'id', 'MakelaarId': 123, 'MakelaarNaam': 'agent'}");
            _fundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), 1))
                .ReturnsAsync(response);
        }
    }
}
