using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq;
using Xunit;

namespace CachingObjects.UnitTests.Services
{
    using CachingObjectsWorkerService.Entities;
    using CachingObjectsWorkerService.ExternalServices;
    using CachingObjectsWorkerService.Models;
    using CachingObjectsWorkerService.Repositories;
    using CachingObjectsWorkerService.Services;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;

    public class TopAgentsCachingServiceTopAgentsRepositoryShould : TopAgentsCachingServiceTestBase
    {
        private readonly TopAgentsCachingService _testing;

        public TopAgentsCachingServiceTopAgentsRepositoryShould()
        {
            _testing = GetServiceInstance();
        }

        [Fact]
        public async Task ShouldSaveTopAgents()
        {
            // arrange
            SetupFundaApiGetObjects();
            SetupRepositoryDeleteAllStatingObject(true);

            var report = new[]
            {
                new TopAgentDetail{ AgentName = "agent1", ObjectCount = 2 },
                new TopAgentDetail{ AgentName = "agent2", ObjectCount = 1 }
            };

            StagingObjectRepositoryMock
                .Setup(repo => repo.GetTopAgentsByObjects(It.IsAny<int>()))
                .ReturnsAsync(report);

            // act
            await _testing.ProsessCachingObjectsAsync();

            // assert
            TopAgentsRepositoryMock
                .Verify(repo => repo.UpdateTopAgents(It.IsAny<string>(), report), Times.Once);
        }
    }
}
