using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public class AppHostTestFixture : IAsyncLifetime
{
    public PlaywrightFixture PlaywrightFixture { get; } = new();

    private DistributedApplication? _app;

    public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>();

        configureBuilder?.Invoke(builder);

        _app = await builder.BuildAsync();

        await _app.StartAsync();

        return _app;
    }

    public async Task InitializeAsync()
    {
        await PlaywrightFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await PlaywrightFixture.DisposeAsync();

        await (_app?.DisposeAsync() ?? ValueTask.CompletedTask);
    }
}
