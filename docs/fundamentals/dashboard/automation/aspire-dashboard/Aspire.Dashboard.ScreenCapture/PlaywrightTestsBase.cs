using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public class PlaywrightTestsBase<TDashboardServerFixture>(AppHostTestFixture appHostTestFixture)
    : IClassFixture<TDashboardServerFixture>, IAsyncDisposable
        where TDashboardServerFixture : AppHostTestFixture
{
    public AppHostTestFixture AppHostTestFixture { get; } = appHostTestFixture;
    public PlaywrightFixture PlaywrightFixture { get; } = appHostTestFixture.PlaywrightFixture;
    public string? DashboardUrl { get; private set; }
    public string DashboardLoginToken { get; private set; } = "";

    private IBrowserContext? _context;

    public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class =>
        AppHostTestFixture.ConfigureAsync<TEntryPoint>(builder =>
        {
            var aspNetCoreUrls = builder.Configuration["ASPNETCORE_URLS"];
            var urls = aspNetCoreUrls is not null ? aspNetCoreUrls.Split(";") : [];

            DashboardUrl = urls.FirstOrDefault();
            DashboardLoginToken = builder.Configuration["AppHost:BrowserToken"] ?? "";

            configureBuilder?.Invoke(builder);
        });

    public async Task RunTestAsync(Func<IPage, Task> test)
    {
        var page = await CreateNewPageAsync();

        try
        {
            await test(page);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task<IPage> CreateNewPageAsync(ScreenSize? size = null)
    {
        _context ??= await PlaywrightFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            ColorScheme = ColorScheme.Dark,
            ScreenSize = size ?? new() { Width = 1350, Height = 500 },
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
