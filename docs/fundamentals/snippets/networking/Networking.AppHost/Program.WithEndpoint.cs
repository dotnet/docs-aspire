public static partial class Program
{
    public static void WithEndpoint(IDistributedApplicationBuilder builder)
    {
        // <withendpoint>
        builder.AddProject<Projects.Networking_Frontend>("frontend")
               .WithEndpoint(
                    endpointName: "frontendEndpoint",
                    callback: static endpoint =>
               {
                   // Configure the endpoint instance directly
               });
        // </withendpoint>
    }
}
