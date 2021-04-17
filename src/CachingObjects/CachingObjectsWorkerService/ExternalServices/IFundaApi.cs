using System.Threading.Tasks;

namespace CachingObjectsWorkerService.ExternalServices
{
    public interface IFundaApi
    {
        Task<dynamic> GetObjects(string location);
    }
}
