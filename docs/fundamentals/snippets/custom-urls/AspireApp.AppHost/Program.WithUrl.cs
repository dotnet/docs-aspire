internal static partial class Program
{
    internal static void WithUrlExample(string[] args)
    {
        // <withurl>
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.AddProject<Projects.AspireApp_Api>("api");

        api.WithUrl($"{api.GetEndpoint("https")}/admin", "Admin Portal");

        builder.Build().Run();
        // </withurl>
    }
}
