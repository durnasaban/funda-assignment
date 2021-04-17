using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    using ExternalServices;
    using Microsoft.Extensions.Logging;
    using Options;
    using Refit;

    public class TopLocationBasedObjectsService : ITopLocationBasedObjectsService
    {
        private readonly IFundaApi _fundaApi;
        private readonly ICollection<string> _objectLocations;
        private readonly int _pageSize;
        private readonly ILogger<TopLocationBasedObjectsService> _logger;

        public TopLocationBasedObjectsService(
            IFundaApi fundaApi,
            IOptions<TopLocationBasedObjectsOptions> topLocationBasedObjectsOptions,
            ILogger<TopLocationBasedObjectsService> logger)
        {
            _fundaApi = fundaApi ?? throw new ArgumentNullException(nameof(fundaApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
                await ProcessCachingObjectsByLocationAsync(location);
            }
        }

        private async Task ProcessCachingObjectsByLocationAsync(string location)
        {
            var currentPage = 1;
            var maxPage = 1;

            while (currentPage <= maxPage)
            {
                var response = await GetObjectsFromApi(location, currentPage);

                if (maxPage == 1)
                {
                    maxPage = response.Paging.AantalPaginas;
                }

                currentPage++;
            }
        }

        private async Task<dynamic> GetObjectsFromApi(string location, int currentPage)
        {
            dynamic response;

            try
            {
                response = await _fundaApi.GetObjects(location, currentPage, _pageSize);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, $"An exception has been thrown while calling funda api. HttpCode = {ex.StatusCode}, Request Message = {ex.RequestMessage}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An exception has been thrown while calling funda api. Page = {currentPage}");
                throw;
            }

            if (response == null)
            {
                throw new Exception($"Api returned null value! Page = {currentPage}");
            }

            return response;
        }
    }
}
