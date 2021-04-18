using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Models;
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;

    public class TopAgentsCachingServiceOptionsShould : TopAgentsCachingServiceTestBase
    {
        public TopAgentsCachingServiceOptionsShould()
        {
            SetupFundaApiGetObjects();
            SetupRepositoryDeleteAllStatingObject();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenEmptyTopLocationBasedObjectsOptions()
        {
            // arrange
            IOptions<TopAgentsCachingOptions> topLocationBasedObjectsOptions = null;

            // act
            Func<TopAgentsCachingService> serviceFunc = () => GetServiceInstance(topLocationBasedObjectsOptions);

            // assert
            serviceFunc.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ShouldConsumeFundaApiOnceGivenJustOneCachingItem()
        {
            // arrange
            var cachingItems = new[]
            {
                new TopAgentsCachingItem
                {
                     Key = "key1",
                     SearchQuery = "searchQuery1",
                }
            };

            var options = GetTopAgentsCachingOptions(1, cachingItems);

            var testing = GetServiceInstance(options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            FundaApiMock
                .Verify(
                api => api.GetObjects(cachingItems[0].SearchQuery, It.IsAny<int>(), It.IsAny<int>()),
                Times.AtLeastOnce);
        }


        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenCachingItems()
        {
            // arrange
            var cachingItems = new[]
            {
                new TopAgentsCachingItem
                {
                     Key = "key1",
                     SearchQuery = "searchQuery1",
                },
                new TopAgentsCachingItem
                {
                     Key = "key2",
                     SearchQuery = "searchQuery2",
                },
                new TopAgentsCachingItem
                {
                     Key = "key3",
                     SearchQuery = "searchQuery3",
                }
            };

            var options = GetTopAgentsCachingOptions(1, cachingItems);

            var testing = GetServiceInstance(options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            foreach (var item in cachingItems)
            {
                FundaApiMock
                    .Verify(
                        api => api.GetObjects(item.SearchQuery, It.IsAny<int>(), It.IsAny<int>()),
                        Times.AtLeastOnce,
                        $"Operation with \"{item.Key}\" can not be verified");
            }
        }

        [Fact]
        public async Task ShouldConsumeFundaApiAtLeastOnceGivenPageSize()
        {
            // arrange
            var pageSize = 1;
            var options = GetDefaultTopAgentsCachingOptions(pageSize);

            var testing = GetServiceInstance(options);

            // act

            await testing.ProsessCachingObjectsAsync();

            // asset
            FundaApiMock
                .Verify(
                api => api.GetObjects(It.IsAny<string>(), It.IsAny<int>(), pageSize),
                Times.AtLeastOnce);
        }
    }
}
