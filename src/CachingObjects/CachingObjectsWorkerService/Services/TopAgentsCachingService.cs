using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    using ExternalServices;
    using Models;
    using Options;

    public class TopAgentsCachingService : ITopAgentsCachingService
    {
        private readonly IFundaApi _fundaApi;
        private readonly ICollection<TopAgentsCachingItem> _cachingItems;
        private readonly int _pageSize;
        private readonly ILogger<TopAgentsCachingService> _logger;

        public TopAgentsCachingService(
            IFundaApi fundaApi,
            IOptions<TopAgentsCachingOptions> topLocationBasedObjectsOptions,
            ILogger<TopAgentsCachingService> logger)
        {
            _fundaApi = fundaApi ?? throw new ArgumentNullException(nameof(fundaApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var options = topLocationBasedObjectsOptions ?? throw new ArgumentNullException(nameof(topLocationBasedObjectsOptions));

            _cachingItems = options.Value.CachingItems;
            _pageSize = options.Value.PageSize;
        }

        public async Task ProsessCachingObjectsAsync()
        {
            foreach (var cachingItem in _cachingItems)
            {
                await ProcessCachingObjectsByLocationAsync(cachingItem);
            }
        }

        private async Task ProcessCachingObjectsByLocationAsync(TopAgentsCachingItem cachingItem)
        {
            var currentPage = 1;
            var maxPage = 1;

            while (currentPage <= maxPage)
            {
                var response = await GetObjectsFromApi(cachingItem.SearchQuery, currentPage);

                // implement mongo
                // Send responses to the mongo
                // implement redis
                // Cache redis reports to redis

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
