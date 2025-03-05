using Azure.Provisioning.Search;

internal static partial class Program
{
    public static void ConfigureSearchInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureSearch("search")
            .ConfigureInfrastructure(infra =>
            {
                var searchService = infra.GetProvisionableResources()
                                         .OfType<SearchService>()
                                         .Single();

                searchService.PartitionCount = 6;
                searchService.ReplicaCount = 3;
                searchService.SearchSkuName = SearchServiceSkuName.Standard3;
                searchService.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
