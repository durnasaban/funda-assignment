using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopAgentsApi.Repositories
{
    using Entities;

    public interface ITopAgentsRepository
    {
        Task<IEnumerable<TopAgentDetail>> GetTopAgents(string key);
    }
}
