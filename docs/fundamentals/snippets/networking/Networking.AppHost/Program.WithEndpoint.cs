public static partial class Program
{
    public static void WithEndpoint(IDistributedApplicationBuilder builder)
    {
        // <withendpoint>
        builder.AddProject<Projects.Networking_ApiService>("apiService")
               .WithEndpoint(
                    endpointName: "FixedAndExternal",
                    callback: static endpoint =>
               {
                   endpoint.Port = 17003;
                   endpoint.AsHttp2()
                           .AsExternal();
               });
        // </withendpoint>
    }
}
