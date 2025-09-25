var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiCacheInvalidationKey = builder.AddParameter("ApiCacheInvalidationKey", secret: true);

var api = builder.AddProject<Projects.AspireApp_Api>("api")
    .WithReference(cache)
    .WaitFor(cache)
    .WithEnvironment("ApiCacheInvalidationKey", apiCacheInvalidationKey)
    .WithHttpCommand(
        path: "/cache/invalidate",
        displayName: "Invalidate cache",
        commandOptions: new HttpCommandOptions()
        {
            Description = """            
                Invalidates the API cache. All cached values are cleared!            
                """,
            PrepareRequest = (context) =>
            {
                var key = apiCacheInvalidationKey.Resource.GetValueAsync(context.CancellationToken);

                context.Request.Headers.Add("X-CacheInvalidation-Key", $"Key: {key}");

                return Task.CompletedTask;
            },
            IconName = "DocumentLightning",
            IsHighlighted = true
        });

builder.Build().Run();
