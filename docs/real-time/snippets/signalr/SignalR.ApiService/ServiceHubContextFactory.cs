namespace SignalR.ApiService;

internal sealed class ServiceHubContextFactory(ServiceManager serviceManager, IMemoryCache cache)
{
    public async Task<ServiceHubContext> GetOrCreateHubContextAsync(string hubName, CancellationToken cancellationToken)
    {
        var context = await cache.GetOrCreateAsync(hubName, async _ =>
        {
            return await serviceManager.CreateHubContextAsync(hubName, cancellationToken);
        });

        return context ?? throw new InvalidOperationException("Failed to create hub context.");
    }
}
