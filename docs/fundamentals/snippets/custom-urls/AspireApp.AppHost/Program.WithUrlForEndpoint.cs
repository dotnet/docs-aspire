internal static partial class Program
{
    internal static void WithUrlForEndpointExample(string[] args)
    {
        // <withurlforendpoint>
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<Projects.AspireApp_Api>("api")
            .WithUrlForEndpoint("https", url =>
            {
                url.DisplayText = "Scalar (HTTPS)";
                url.Url += "/scalar";
            });

        builder.Build().Run();
        // </withurlforendpoint>
    }
}
