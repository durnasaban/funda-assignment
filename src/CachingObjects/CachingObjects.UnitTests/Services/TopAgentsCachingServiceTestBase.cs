using Microsoft.Extensions.Options;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Models;
    using CachingObjectsWorkerService.Options;

    public class TopAgentsCachingServiceTestBase
    {
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
