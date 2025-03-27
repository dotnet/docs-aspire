using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Playwright;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public sealed class PlaywrightTestsBase<TDashboardServerFixture>(AppHostTestFixture appHostTestFixture)
    : IClassFixture<TDashboardServerFixture>, IAsyncDisposable
        where TDashboardServerFixture : AppHostTestFixture
{
    public AppHostTestFixture AppHostTestFixture { get; } = appHostTestFixture;
    public PlaywrightFixture PlaywrightFixture { get; } = appHostTestFixture.PlaywrightFixture;

    private IBrowserContext? _context;

    public Task<DistributedApplication> ConfigureAsync<TEntryPoint>(
        Action<IDistributedApplicationTestingBuilder>? configureBuilder = null) where TEntryPoint : class
    {
        return AppHostTestFixture.ConfigureAsync<TEntryPoint>(configureBuilder);
    }

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

    private async Task<IPage> CreateNewPageAsync()
    {
        _context ??= await PlaywrightFixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
            BaseURL = AppHostTestFixture.DashboardApp.FrontendSingleEndPointAccessor().GetResolvedAddress()
        });

        return await _context.NewPageAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }
    }
}
