using Microsoft.Extensions.Options;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Options;

    public class TopLocationBasedObjectsServiceTestBase
    {
        protected IOptions<TopLocationBasedObjectsOptions> GetTopLocationBasedObjectOptions(
            int pageSize = 1, 
            params string[] locations)
        {
            var options = new TopLocationBasedObjectsOptions
            {
                Locations = locations,
                PageSize = pageSize
            };

            return Options.Create(options);
        }
    }
}
