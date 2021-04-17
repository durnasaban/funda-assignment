using Refit;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.ExternalServices
{
    public interface IFundaApi
    {
        [Get("/?type=koop&zo=/{searchQuery}/&page={currentPage}&pagesize={pageSize}")]
        Task<dynamic> GetObjects(string searchQuery, int currentPage, int pageSize);
    }
}
