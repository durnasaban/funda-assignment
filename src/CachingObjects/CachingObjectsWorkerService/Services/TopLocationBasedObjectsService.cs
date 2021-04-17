using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    using ExternalServices;
    using Options;

    public class TopLocationBasedObjectsService : ITopLocationBasedObjectsService
    {
        private readonly IFundaApi _fundaApi;
        private readonly ICollection<string> _objectLocations;
        private readonly int _pageSize;

        public TopLocationBasedObjectsService(
            IFundaApi fundaApi,
            IOptions<TopLocationBasedObjectsOptions> topLocationBasedObjectsOptions)
        {
            _fundaApi = fundaApi ?? throw new ArgumentNullException(nameof(fundaApi)); ;

            var options = topLocationBasedObjectsOptions ?? throw new ArgumentNullException(nameof(topLocationBasedObjectsOptions));

            _objectLocations = options.Value.Locations != null && options.Value.Locations.Count != 0 ?
                                options.Value.Locations :
                                throw new ArgumentNullException(nameof(options.Value.Locations));

            _pageSize = options.Value.PageSize != default ?
                            options.Value.PageSize :
                            throw new ArgumentException(nameof(options.Value.PageSize));

        }

        public async Task ProsessCachingObjectsAsync()
        {
            foreach (var location in _objectLocations)
            {
                var response = await _fundaApi.GetObjects(location, 1, _pageSize);
            }
        }
    }
}
