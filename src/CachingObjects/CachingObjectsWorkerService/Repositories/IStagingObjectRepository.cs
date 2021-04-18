using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Models;
    using Entities;

    public interface IStagingObjectRepository
    {
        Task<bool> DeleteAllStagingObjects();

        Task CreateStagingObjects(ICollection<StagingObject> stagingObjects);

        Task<IEnumerable<TopAgentDetail>> GetTopAgentsByObjects(int count);
    }
}
