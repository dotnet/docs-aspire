using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddLifecycleHook<LifecycleLogger>();

builder.Build().Run();

internal sealed class LifecycleLogger(ILogger<LifecycleLogger> logger)
    : IDistributedApplicationLifecycleHook
{
    public Task BeforeStartAsync(
        DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("1. BeforeStartAsync");
        return Task.CompletedTask;
    }

    public Task AfterEndpointsAllocatedAsync(
        DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("2. AfterEndpointsAllocatedAsync");
        return Task.CompletedTask;
    }

    public Task AfterResourcesCreatedAsync(
        DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("3. AfterResourcesCreatedAsync");
        return Task.CompletedTask;
    }
}
