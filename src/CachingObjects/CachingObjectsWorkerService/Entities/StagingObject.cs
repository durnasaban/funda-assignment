using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CachingObjectsWorkerService.Entities
{
    public class StagingObject
    {
        [BsonId]
        public string Id { get; set; }

        public string ObjectId { get; set; }

        public string AgentId { get; set; }

        public string AgentName { get; set; }
    }
}
