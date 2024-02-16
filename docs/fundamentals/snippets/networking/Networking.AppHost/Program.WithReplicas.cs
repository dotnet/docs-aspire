public static partial class Program
{
    public static void WithReplicas(IDistributedApplicationBuilder builder)
    {
        // <withreplicas>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithHttpEndpoint(hostPort: 5066)
               .WithReplicas(2);
        // </withreplicas>
    }
}
