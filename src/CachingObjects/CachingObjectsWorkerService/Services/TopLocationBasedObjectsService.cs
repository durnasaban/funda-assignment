using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace CachingObjectsWorkerService.Services
{
    using Options;

    public class TopLocationBasedObjectsService : ITopLocationBasedObjectsService
    {
        private ICollection<string> _objectLocations;

        public TopLocationBasedObjectsService(IOptions<TopLocationBasedObjectsOptions> topLocationBasedObjectsOptions)
        {
            var options = topLocationBasedObjectsOptions ?? throw new ArgumentNullException(nameof(topLocationBasedObjectsOptions));

            _objectLocations = options.Value.Locations != null && options.Value.Locations.Count != 0 ? 
                                options.Value.Locations : 
                                throw new ArgumentNullException(nameof(options.Value.Locations));
        }
    }
}
