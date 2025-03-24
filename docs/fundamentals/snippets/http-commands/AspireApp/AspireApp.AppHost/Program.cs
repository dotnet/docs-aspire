using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// TODO: Delete when bug is fixed
//       https://github.com/dotnet/aspire/issues/8270
builder.Services.AddHttpClient();

var cache = builder.AddRedis("cache");

var apiCacheInvalidationKey = builder.AddParameter("ApiCacheInvalidationKey", secret: true);

var api = builder.AddProject<Projects.AspireApp_Api>("api")
    .WithReference(cache)
    .WaitFor(cache)
    .WithEnvironment("ApiCacheInvalidationKey", apiCacheInvalidationKey)
    .WithHttpCommand(
        path: "/cache/invalidate",
        displayName: "Invalidate cache",
        displayDescription: """
            Invalidates the API cache. All cached values are cleared!
            """,
        configureRequest: (context) =>
        {
            var key = apiCacheInvalidationKey.Resource.Value;

            context.Request.Headers.Add("X-CacheInvalidation-Key", $"Key: {key}");

            return Task.CompletedTask;
        },
        iconName: "DocumentLightning",
        isHighlighted: false);

builder.Build().Run();
