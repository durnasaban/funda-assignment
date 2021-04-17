using Refit;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.ExternalServices
{
    public interface IFundaApi
    {
        [Get("/?type=koop&zo=/{location}/&page={currentPage}&pagesize={pageSize}")]
        Task<dynamic> GetObjects(string location, int currentPage, int pageSize);
    }
}
