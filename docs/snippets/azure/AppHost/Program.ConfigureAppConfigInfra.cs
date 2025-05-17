using Azure.Provisioning.AppConfiguration;

internal static partial class Program
{
    public static void ConfigureAppConfigInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureAppConfiguration("config")
            .ConfigureInfrastructure(infra =>
            {
                var appConfigStore = infra.GetProvisionableResources()
                                          .OfType<AppConfigurationStore>()
                                          .Single();

                appConfigStore.SkuName = "Free";
                appConfigStore.EnablePurgeProtection = true;
                appConfigStore.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
