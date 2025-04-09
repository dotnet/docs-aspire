internal static partial class Program
{
    internal static void WithUrlsExample(string[] args)
    {
        // <withurls>
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddProject<Projects.AspireApp_Api>("api")
            .WithUrls(context =>
            {
                foreach (var tuple in context.Urls
                    .Select(url => (Url: url, Uri: new Uri(url.Url)))
                    .OrderByDescending(_ => _.Uri.Scheme is "https")
                    .Select((pair, index) => (pair.Url, pair.Uri.Scheme, Index: index)))
                {
                    var (url, scheme, index) = tuple;

                    // Order HTTPS first.
                    var order = context.Urls.Count - 1 - index;

                    url.DisplayText = $"{index + 1}. {scheme.ToUpper()}";
                    url.DisplayOrder = order;
                }
            });

        builder.Build().Run();
        // </withurls>
    }
}
