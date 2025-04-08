internal static partial class Program
{
    internal static void WithUrlExample(string[] args)
    {
        // <withurl>
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.AddProject<Projects.AspireApp_Api>("api");

        var adminUrl = ReferenceExpression.Create(
            $"{api.GetEndpoint("https")}/admin");

        api.WithUrl(adminUrl, "Admin Portal");

        builder.Build().Run();
        // </withurl>
    }
}
