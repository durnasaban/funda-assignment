using System.Collections.Generic;

namespace CachingObjectsWorkerService.Options
{
    public class TopLocationBasedObjectsOptions : ObjectsOptionsBase
    {
        public const string TopLocationBasedObjects = "TopLocationBasedObjects";

        public ICollection<string> Locations { get; set; }
    }
}
