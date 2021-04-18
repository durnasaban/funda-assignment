using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using CachingObjectsWorkerService.Data;
    using Entities;
    using MongoDB.Driver;
    using System;

    public class StagingObjectRepository : IStagingObjectRepository
    {
        private readonly IFundaContext _context;

        public StagingObjectRepository(IFundaContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> DeleteAllStagingObjects()
        {
            var filter = Builders<StagingObject>.Filter.Empty;

            var deleteResult = await _context
                                        .StagingObjects
                                        .DeleteManyAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public Task CreateStagingObjects(ICollection<StagingObject> stagingObjects)
        {
            throw new System.NotImplementedException();
        }
    }
}
