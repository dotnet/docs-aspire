public static partial class Program
{
    public static void FilterEndpoint(IDistributedApplicationBuilder builder)
    {
        // <filter>
        builder.AddProject<Projects.Networking_ApiService>("apiservice")
            .WithHttpsEndpoint() // Adds a default "https" endpoint
            .WithHttpsEndpoint(port: 19227, name: "admin")
            .WithEndpointsInEnvironment(
                filter: static endpoint =>
                {
                    return endpoint.Name is not "admin";
                });
        // </filter>
    }
}
