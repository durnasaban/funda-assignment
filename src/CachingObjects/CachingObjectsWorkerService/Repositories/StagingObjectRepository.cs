using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Data;
    using Entities;

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

        public async Task CreateStagingObjects(ICollection<StagingObject> stagingObjects) =>
            await _context
                     .StagingObjects
                     .InsertManyAsync(stagingObjects);
    }
}
