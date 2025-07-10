using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

cache.OnResourceReady(static (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("5. OnResourceReady");

        return Task.CompletedTask;
    });

cache.OnInitializeResource(
    static (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. OnInitializeResource");

        return Task.CompletedTask;
    });

cache.OnBeforeResourceStarted(
    static (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("4. OnBeforeResourceStarted");


        return Task.CompletedTask;
    });

cache.OnResourceEndpointsAllocated(
    static (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. OnResourceEndpointsAllocated");

        return Task.CompletedTask;
    });

cache.OnConnectionStringAvailable(
    static (resource, @event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("3. OnConnectionStringAvailable");

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
