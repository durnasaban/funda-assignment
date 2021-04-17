using FluentAssertions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Options;
    using CachingObjectsWorkerService.Services;

    public class TopLocationBasedObjectsServiceLocationsShould
    {
        [Fact]
        public void ThrowArgumentNullExceptionGivenEmptyTopLocationBasedObjectsOptions()
        {
            // arrange
            IOptions<TopLocationBasedObjectsOptions> topLocationBasedObjectsOptions = null;

            // act
            Func<TopLocationBasedObjectsService> serviceFunc = () => new TopLocationBasedObjectsService(topLocationBasedObjectsOptions);

            // assert
            serviceFunc.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowArgumentNullExceptionGivenNullLocations()
        {
            // arrange
            var emptyLocations = new List<string>();

            // arrange
            Func<TopLocationBasedObjectsService> serviceFuncWithNullLocations = () => 
                new TopLocationBasedObjectsService(GetTopLocationBasedObjectOptions());

            Func<TopLocationBasedObjectsService> serviceFuncWithEmptyLocations = () => 
                new TopLocationBasedObjectsService(GetTopLocationBasedObjectOptions(emptyLocations));

            // assert
            serviceFuncWithNullLocations.Should().Throw<ArgumentNullException>();
            serviceFuncWithEmptyLocations.Should().Throw<ArgumentNullException>();
        }

        private static IOptions<TopLocationBasedObjectsOptions> GetTopLocationBasedObjectOptions(ICollection<string> locations = null)
        {
            var options = new TopLocationBasedObjectsOptions
            {
                Locations = locations
            };

            return Options.Create(options);
        }
    }
}
