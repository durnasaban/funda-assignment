using System.Collections.Generic;

namespace CachingObjectsWorkerService.Options
{
    using Models;

    public class TopAgentsCachingOptions
    {
        public const string TopAgentsCaching = "TopAgentsCaching";

        public int ExecutionPeriodInMinutes { get; set; }

        public int PageSize { get; set; }

        public ICollection<TopAgentsCachingItem> CachingItems { get; set; }
    }
}
