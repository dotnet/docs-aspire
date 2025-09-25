internal static partial class Program
{
    public static void AllowAnonymous(string[] args)
    {
        // <anonymous>
        var builder = DistributedApplication.CreateBuilder(args);

        var web = builder.AddProject<Projects.Web>("web");

        builder.AddDevTunnel("public-api")
               .WithReference(web)
               .WithAnonymousAccess();

        // After adding all resources, run the app...
        // </anonymous>
    }
}