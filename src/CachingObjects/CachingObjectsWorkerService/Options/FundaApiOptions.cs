namespace CachingObjectsWorkerService.Options
{
    public class FundaApiOptions
    {
        public const string FundaApi = "FundaApi";

        public string BaseUrl { get; set; }

        public string Query { get; set; }

        public string Key { get; set; }
    }
}
