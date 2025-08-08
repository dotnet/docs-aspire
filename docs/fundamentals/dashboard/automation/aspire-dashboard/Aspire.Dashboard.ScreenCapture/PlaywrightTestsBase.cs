using Aspire.Hosting.ApplicationModel;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Dashboard.ScreenCapture;

public class PlaywrightTestsBase<TDashboardServerFixture>(AppHostTestFixture appHostTestFixture)
    : IClassFixture<TDashboardServerFixture>, IAsyncDisposable
        where TDashboardServerFixture : AppHostTestFixture
{
    public AppHostTestFixture AppHostTestFixture { get; } = appHostTestFixture;
    public PlaywrightFixture PlaywrightFixture { get; } = appHostTestFixture.PlaywrightFixture;
    public string? DashboardUrl { get; internal set; }
    public string DashboardLoginToken { get; private set; } = "";

    private IBrowserContext? _context;

    public async Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        string[]? args = null,
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
    {
        var app = await AppHostTestFixture.ConfigureAsync<TEntryPoint>(args, builder =>
        {
            DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? "";

            builder.Eventing.Subscribe<ResourceEndpointsAllocatedEvent>((@event, _) =>
            {
                if (@event.Resource.TryGetUrls(out var urls))
                {
                    DashboardUrl = urls.FirstOrDefault()?.ToString() ?? "";
                }

                return Task.CompletedTask;
            });

            configureBuilder?.Invoke(builder);
        });

        var server = app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        if (addresses is not null)
        {
            
        }
        var hostUrl = app.GetEndpoint("aspire-dashboard");
        DashboardUrl = hostUrl.ToString();

        return app;
    }

    public async Task InteractWithPageAsync(Func<IPage, Task> test, ViewportSize? size = null)
    {
        var page = await CreateNewPageAsync(size);

        try
        {
            await test(page);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error during test interaction: {ex.Message}");

            throw;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task<IPage> CreateNewPageAsync(ViewportSize? size = null)
    {
        _context = await PlaywrightFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ColorScheme = ColorScheme.Dark,
            ViewportSize = size,
            BaseURL = DashboardUrl
        });

        return await _context.NewPageAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_context is not null)
        {
            await _context.DisposeAsync();
        }
    }
}
