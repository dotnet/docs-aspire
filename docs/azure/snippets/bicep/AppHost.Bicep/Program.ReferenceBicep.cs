using Aspire.Hosting.Azure;

internal static partial class Program
{
    public static void AddBicepFileReference(IDistributedApplicationBuilder builder)
    {
        // <addfile>
        builder.AddBicepTemplate(
            name: "storage",
            bicepFile: "../infra/storage.bicep");
        // </addfile>
    }
}
