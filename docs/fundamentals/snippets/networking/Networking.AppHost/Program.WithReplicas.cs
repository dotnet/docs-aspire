namespace Networking.AppHost;

public static partial class Program
{
    public static void WithReplicas(DistributedApplicationBuilder builder)
    {
        // <withreplicas>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithServiceBinding(hostPort: 5066, scheme: "http")
               .WithReplicas(2);
        // </withreplicas>
    }
}
