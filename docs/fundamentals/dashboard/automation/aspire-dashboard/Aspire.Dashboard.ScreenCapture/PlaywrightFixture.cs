using Microsoft.Playwright;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public class PlaywrightFixture : IAsyncLifetime
{
    public IBrowser Browser { get; set; } = null!;

    public async Task InitializeAsync()
    {
        // Default timeout of 5000 ms could time out on slow CI servers.
        Assertions.SetDefaultExpectTimeout(30_000);

        Browser = await PlaywrightProvider.CreateBrowserAsync(new()
        {
            Headless = false
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
    }

    public async Task GotoHomeAsync(IPage page)
    {
        await page.GotoAsync("/");
    }
}
