using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public class AppHostTestFixture : IAsyncLifetime
{
    public PlaywrightFixture PlaywrightFixture { get; } = new();

    public DistributedApplication? App { get; private set; }

    public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<TEntryPoint>(
            args: args ?? [],
            configureBuilder: static (options, _) =>
            {
                options.DisableDashboard = false;
            });

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        configureBuilder?.Invoke(builder);

        App = await builder.BuildAsync();

        await App.StartAsync();

        return App;
    }

    public async Task InitializeAsync()
    {
        await PlaywrightFixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await PlaywrightFixture.DisposeAsync();

        await (App?.DisposeAsync() ?? ValueTask.CompletedTask);
    }
}
