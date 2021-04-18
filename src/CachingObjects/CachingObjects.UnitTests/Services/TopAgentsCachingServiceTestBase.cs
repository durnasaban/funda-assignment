using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Models;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Repositories;
    using CachingObjectsWorkerService.Services;
    using Newtonsoft.Json.Linq;

    public class TopAgentsCachingServiceTestBase
    {
        protected readonly Mock<IFundaApi> FundaApiMock;
        protected readonly Mock<IStagingObjectRepository> StagingObjectRepositoryMock;
        protected readonly Mock<ITopAgentsRepository> TopAgentsRepositoryMock;
        protected readonly Mock<ILogger<TopAgentsCachingService>> LoggerMock;

        public TopAgentsCachingServiceTestBase()
        {
            FundaApiMock = new Mock<IFundaApi>();
            StagingObjectRepositoryMock = new Mock<IStagingObjectRepository>();
            TopAgentsRepositoryMock = new Mock<ITopAgentsRepository>();
            LoggerMock = new Mock<ILogger<TopAgentsCachingService>>();
        }

        protected TopAgentsCachingService GetServiceInstance(IOptions<TopAgentsCachingOptions> options)
        {
            return new TopAgentsCachingService(
                FundaApiMock.Object,
                options,
                LoggerMock.Object,
                StagingObjectRepositoryMock.Object,
                TopAgentsRepositoryMock.Object);
        }


        protected TopAgentsCachingService GetServiceInstance() => GetServiceInstance(GetDefaultTopAgentsCachingOptions());

        protected void SetupRepositoryDeleteAllStatingObject(bool result = true) =>
         StagingObjectRepositoryMock
             .Setup(repo => repo.DeleteAllStagingObjects())
             .ReturnsAsync(result);

        protected void SetupFundaApiGetObjects(int totalCount = 1, int pageSize = 1, string id = "id", int agentId = 1, string agentName = "agent")
        {
            var response = JObject.Parse(@"{'Paging': { 'AantalPaginas' : '" + totalCount + "' }, 'Objects':[{'Id':'" + id + "', 'MakelaarId': " + agentId + ", 'MakelaarNaam': '" + agentName + "'}]}");

            FundaApiMock
                .Setup(api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize))
                .ReturnsAsync(response);
        }

        protected IOptions<TopAgentsCachingOptions> GetTopAgentsCachingOptions(
            int pageSize,
            params TopAgentsCachingItem[] cachingItems)
        {
            var options = new TopAgentsCachingOptions
            {
                CachingItems = cachingItems,
                PageSize = pageSize
            };

            return Options.Create(options);
        }

        protected IOptions<TopAgentsCachingOptions> GetDefaultTopAgentsCachingOptions(int pageSize = 1)
        {
            var cachingItems = new[]
            {
                new TopAgentsCachingItem
                {
                     Key = "key",
                      SearchQuery = "searchKey",
                       TopAgentCount = 10
                }
            };

            return GetTopAgentsCachingOptions(pageSize, cachingItems);
        }
    }
}
