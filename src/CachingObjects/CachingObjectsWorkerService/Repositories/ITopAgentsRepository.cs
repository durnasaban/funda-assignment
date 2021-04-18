using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Models;

    public interface ITopAgentsRepository
    {
        Task UpdateTopAgents(string key, IEnumerable<TopAgentDetail> agentDetails);
    }
}
