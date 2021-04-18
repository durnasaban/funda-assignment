using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace CachingObjectsWorkerService.Data
{
    using Entities;
    using Options;

    public class FundaContext : IFundaContext
    {
        public FundaContext(IOptions<MongoDbSettingsOptions> settingsOptions)
        {
            if (settingsOptions == null)
            {
                throw new ArgumentNullException(nameof(settingsOptions));
            }

            var settings = settingsOptions.Value;

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            StagingObjects = database.GetCollection<StagingObject>(settings.CollectionName);
        }

        public IMongoCollection<StagingObject> StagingObjects { get; }
    }
}
