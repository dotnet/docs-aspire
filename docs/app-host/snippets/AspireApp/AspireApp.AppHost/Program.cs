using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice");

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

_ = builder.Eventing.Subscribe<BeforeStartEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("1. BeforeStartEvent");

        return Task.CompletedTask;
    });

_ = builder.Eventing.Subscribe<AfterEndpointsAllocatedEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("2. AfterEndpointsAllocatedEvent");

        return Task.CompletedTask;
    });

_ = builder.Eventing.Subscribe<AfterResourcesCreatedEvent>(
    static (@event, cancellationToken) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("3. AfterResourcesCreatedEvent");

        return Task.CompletedTask;
    });

builder.Build().Run();
