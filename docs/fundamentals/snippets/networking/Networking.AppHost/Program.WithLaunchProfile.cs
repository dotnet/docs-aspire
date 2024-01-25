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
               .WithServiceBinding(hostPort: 5066, scheme: "http")
               .WithServiceBinding(hostPort: 7239, scheme: "https");
        // </verbose>
    }
}
