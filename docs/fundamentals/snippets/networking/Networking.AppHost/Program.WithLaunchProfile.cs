public static partial class Program
{
    public static void WithLaunchProfile(IDistributedApplicationBuilder builder)
    {
        // <withlaunchprofile>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithLaunchProfile("https");
        // </withlaunchprofile>

        // <verbose>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithHttpEndpoint(hostPort: 5066)
               .WithHttpsEndpoint(hostPort: 7239);
        // </verbose>
    }
}
