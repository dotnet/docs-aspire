internal static partial class Program
{
    internal static void WithUrlsExample(string[] args)
    {
        // <withurls>
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<Projects.AspireApp_Api>("api")
            .WithUrls(context =>
            {
                foreach (var url in context.Urls)
                {
                    if (string.IsNullOrEmpty(url.DisplayText))
                    {
                        url.DisplayText = $"API ({url.Endpoint?.Scheme?.ToUpper()})";
                    }
                }
            });

        builder.Build().Run();
        // </withurls>
    }
}
