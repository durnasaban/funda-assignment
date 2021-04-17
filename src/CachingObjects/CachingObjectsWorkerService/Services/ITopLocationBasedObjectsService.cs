using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Services
{
    public interface ITopLocationBasedObjectsService
    {
        Task ProsessCachingObjectsAsync();
    }
}
