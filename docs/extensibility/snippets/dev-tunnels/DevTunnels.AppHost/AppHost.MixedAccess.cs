internal static partial class Program
{
    public static void MixedAccess(string[] args)
    {
        // <mixedaccess>
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.AddProject<Projects.ApiService>("api");

        builder.AddDevTunnel("mixed-access")
               .WithReference(api.GetEndpoint("public"), allowAnonymous: true)
               .WithReference(api.GetEndpoint("admin")); // Requires authentication

        // After adding all resources, run the app...
        // </mixedaccess>
    }
}