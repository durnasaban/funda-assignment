using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TopAgentsApi.Repositories
{
    using Entities;

    public class TopAgentsRepository : ITopAgentsRepository
    {
        private readonly IDistributedCache _redisCache;

        public TopAgentsRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<IEnumerable<TopAgentDetail>> GetTopAgents(string key)
        {
            var topAgents = await _redisCache.GetStringAsync(key);

            if (topAgents == null)
                return Enumerable.Empty<TopAgentDetail>();

            return JsonConvert.DeserializeObject<IEnumerable<TopAgentDetail>>(topAgents);
        }
    }
}
