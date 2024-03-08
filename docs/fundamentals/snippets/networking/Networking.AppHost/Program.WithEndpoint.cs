public static partial class Program
{
    public static void WithEndpoint(IDistributedApplicationBuilder builder)
    {
        // <withendpoint>
        builder.AddProject<Projects.Networking_ApiService>("apiService")
               .WithEndpoint(
                    endpointName: "admin",
                    callback: static endpoint =>
               {
                   endpoint.Port = 17003;
                   endpoint.UriScheme = "http";
                   endpoint.Transport = "http";
               });
        // </withendpoint>
    }
}
