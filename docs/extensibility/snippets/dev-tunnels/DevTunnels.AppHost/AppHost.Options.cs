using Aspire.Hosting.DevTunnels;

internal static partial class Program
{
    public static void Options(string[] args)
    {
        // <options>
        var builder = DistributedApplication.CreateBuilder(args);

        var web = builder.AddProject<Projects.Web>("web");

        var options = new DevTunnelOptions
        {
            AllowAnonymous = false,
            Description = "Shared QA validation tunnel",
            Labels = ["qa", "validation"],
        };

        var customTunnel = builder.AddDevTunnel(
                name: "qa",
                tunnelId: "qa-shared",
                options: options
            )
            .WithReference(web);

        // After adding all resources, run the app...
        // </options>
    }
}