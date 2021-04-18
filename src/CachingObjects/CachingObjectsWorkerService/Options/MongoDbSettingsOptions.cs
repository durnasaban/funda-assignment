namespace CachingObjectsWorkerService.Options
{
    public class MongoDbSettingsOptions
    {
        public const string MongoDbSettings = "MongoDbSettings";

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }
    }
}
