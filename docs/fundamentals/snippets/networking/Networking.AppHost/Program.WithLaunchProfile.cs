public static partial class Program
{
    public static void WithLaunchProfile(IDistributedApplicationBuilder builder)
    {
        // <verbose>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithHttpEndpoint(port: 5066)
               .WithHttpsEndpoint(port: 7239);
        // </verbose>
    }
}
