using Aspire.Hosting.Azure;

internal static partial class Program
{
    public static void AddParameterToBicepReference(IDistributedApplicationBuilder builder)
    {
        // <addparameter>
        var region = builder.AddParameter("region");

        builder.AddBicepTemplate("storage", "../infra/storage.bicep")
               .WithParameter("region", region)
               .WithParameter("storageName", "app-storage")
               .WithParameter(AzureBicepResource.KnownParameters.PrincipalId)
               .WithParameter(AzureBicepResource.KnownParameters.PrincipalType)
               .WithParameter("tags", ["latest","dev"]);
        // </addparameter>
    }
}
