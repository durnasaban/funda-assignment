using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Models;

    public class TopAgentsRepository : ITopAgentsRepository
    {
        private readonly IDistributedCache _redisCache;

        public TopAgentsRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task UpdateTopAgents(string key, IEnumerable<TopAgentDetail> agentDetails)
        {
            var serializedValue = JsonConvert.SerializeObject(agentDetails);

            await _redisCache.SetStringAsync(key, serializedValue);
        }
    }
}
