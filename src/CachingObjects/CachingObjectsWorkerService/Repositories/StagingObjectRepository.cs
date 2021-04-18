using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CachingObjectsWorkerService.Repositories
{
    using Models;
    using Data;
    using Entities;
    using MongoDB.Bson;

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

        public async Task<IEnumerable<TopAgentDetail>> GetTopAgentsByObjects(int count)
        {
            var group = new BsonDocument
            {
                { "$group",
                    new BsonDocument
                    {
                        { "_id",
                            new BsonDocument {
                                {nameof(StagingObject.AgentId), $"${nameof(StagingObject.AgentId)}" },
                                {nameof(StagingObject.AgentName), $"${nameof(StagingObject.AgentName)}"}, }
                        },
                        { nameof(TopAgentDetail.ObjectCount),
                            new BsonDocument ("$count", $"${nameof(StagingObject.ObjectId)}")
                        },
                    }
                }
            };

            var sort = new BsonDocument
            {
                {
                    "$sort", new BsonDocument(nameof(TopAgentDetail.ObjectCount), -1 )
                }
            };

            var limit = new BsonDocument("$limit", count);

            var result = await _context
                            .StagingObjects
                            .Aggregate()
                                .AppendStage<BsonDocument>(group)
                                .AppendStage<BsonDocument>(sort)
                                .AppendStage<BsonDocument>(limit)
                            .ToListAsync();

            return result.Select(r =>
                            new TopAgentDetail
                            {
                                AgentName = r.GetValue(1).ToString(),
                                ObjectCount = (int)r.GetValue(2)
                            });
        }
    }
}
