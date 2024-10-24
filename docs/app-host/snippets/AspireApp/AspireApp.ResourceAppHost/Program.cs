using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

_ = builder.Eventing.Subscribe<BeforeResourceStartedEvent>(
    cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. BeforeResourceStartedEvent");

        return Task.CompletedTask;
    });

_ = builder.Eventing.Subscribe<ConnectionStringAvailableEvent>(
    cache.Resource,
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. ConnectionStringAvailableEvent");

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
