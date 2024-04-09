public static partial class Program
{
    public static void WithReplicas(IDistributedApplicationBuilder builder)
    {
        // <withreplicas>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithHttpEndpoint(port: 5066)
               .WithReplicas(2);
        // </withreplicas>
    }
}
