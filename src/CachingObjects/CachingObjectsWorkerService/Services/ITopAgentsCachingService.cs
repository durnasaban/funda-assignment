using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    public interface ITopAgentsCachingService
    {
        Task ProsessCachingObjectsAsync();
    }
}
