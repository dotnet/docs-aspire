using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

builder.Eventing.Subscribe<ResourceReadyEvent>(
    cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("4. ResourceReadyEvent");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<InitializeResourceEvent>(cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. InitializeResourceEvent");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<BeforeResourceStartedEvent>(
    cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("3. BeforeResourceStartedEvent");

        return Task.CompletedTask;
    });

builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(
    cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. ConnectionStringAvailableEvent");

        return Task.CompletedTask;
    });

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
