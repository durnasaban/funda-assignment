using MongoDB.Driver;

namespace CachingObjectsWorkerService.Data
{
    using Entities;

    public interface IFundaContext
    {
        public IMongoCollection<StagingObject> StagingObjects { get; }
    }
}
