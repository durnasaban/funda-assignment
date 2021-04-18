using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Models;

    public class TopAgentsRepository : ITopAgentsRepository
    {
        public Task UpdateTopAgents(string key, IEnumerable<TopAgentDetail> agentDetails)
        {
            throw new System.NotImplementedException();
        }
    }
}
