internal static partial class Program
{
    public static void AddBicepAndGetOutput(IDistributedApplicationBuilder builder)
    {
        // <getoutput>
        var storage = builder.AddBicepTemplate(
                name: "storage",
                bicepFile: "../infra/storage.bicep"
            );

        var endpoint = storage.GetOutput("endpoint");
        // </getoutput>
    }
}
