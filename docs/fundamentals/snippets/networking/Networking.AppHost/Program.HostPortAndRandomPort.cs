public static partial class Program
{
    public static void HostPortWithRandomServicePort(IDistributedApplicationBuilder builder)
    {
        // <hostport>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithServiceBinding(hostPort: 5066, scheme: "http");
        // </hostport>
    }
}
