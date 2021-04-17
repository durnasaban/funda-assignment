using Microsoft.Extensions.Options;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Options;

    public class TopAgentsCachingServiceTestBase
    {
        protected IOptions<TopAgentsCachingOptions> GetTopLocationBasedObjectOptions(
            int pageSize = 1, 
            params string[] locations)
        {
            var options = new TopAgentsCachingOptions
            {
                Locations = locations,
                PageSize = pageSize
            };

            return Options.Create(options);
        }
    }
}
