using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    using CachingObjectsWorkerService.Entities;
    using ExternalServices;
    using Models;
    using Options;
    using Repositories;

    public class TopAgentsCachingService : ITopAgentsCachingService
    {
        private readonly IFundaApi _fundaApi;
        private readonly IStagingObjectRepository _stagingObjectRepository;        
        private readonly ILogger<TopAgentsCachingService> _logger;
        private readonly ICollection<TopAgentsCachingItem> _cachingItems;
        private readonly int _pageSize;

        public TopAgentsCachingService(
            IFundaApi fundaApi,
            IOptions<TopAgentsCachingOptions> topLocationBasedObjectsOptions,
            ILogger<TopAgentsCachingService> logger,
            IStagingObjectRepository stagingObjectRepository)
        {
            _fundaApi = fundaApi ?? throw new ArgumentNullException(nameof(fundaApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stagingObjectRepository = stagingObjectRepository ?? throw new ArgumentNullException(nameof(stagingObjectRepository));

            var options = topLocationBasedObjectsOptions ?? throw new ArgumentNullException(nameof(topLocationBasedObjectsOptions));

            _cachingItems = options.Value.CachingItems;
            _pageSize = options.Value.PageSize;            
        }

        public async Task ProsessCachingObjectsAsync()
        {
            foreach (var cachingItem in _cachingItems)
            {
                var deleteResult = await _stagingObjectRepository.DeleteAllStagingObjects();

                if (!deleteResult)
                {
                    throw new Exception("Staging Object Table couldn't be cleaned up!");
                }

                await GetAndSaveObjectsAsync(cachingItem);

                var topAgents = await _stagingObjectRepository.GetTopAgentsByObjects(cachingItem.TopAgentCount);
            }
        }

        private async Task GetAndSaveObjectsAsync(TopAgentsCachingItem cachingItem)
        {
            var currentPage = 1;
            var maxPage = 1;

            while (currentPage <= maxPage)
            {
                var response = await GetObjectsFromApi(cachingItem.SearchQuery, currentPage);

                await _stagingObjectRepository.CreateStagingObjects(GetStagingObjects(response));

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

        private static List<StagingObject> GetStagingObjects(dynamic response)
        {
            var stagingObjects = new List<StagingObject>();

            foreach (var items in response.Objects)
            {
                stagingObjects.Add(new StagingObject
                {
                    Id = items.Id,
                    AgentId = items.MakelaarId,
                    AgentName = items.MakelaarNaam
                });
            }

            return stagingObjects;
        }
    }
}
