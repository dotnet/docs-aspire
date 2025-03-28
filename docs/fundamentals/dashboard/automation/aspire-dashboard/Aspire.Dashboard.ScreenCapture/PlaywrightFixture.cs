using System.Diagnostics;
using Microsoft.Playwright;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

public class PlaywrightFixture : IAsyncLifetime
{
    private static bool IsDebugging => Debugger.IsAttached;
    private static bool IsHeadless => IsDebugging is false;

    private IPlaywright? _playwright;

    public IBrowser Browser { get; set; } = null!;

    public async Task InitializeAsync()
    {
        Assertions.SetDefaultExpectTimeout(10_000);

        _playwright = await Playwright.CreateAsync();

        var options = new BrowserTypeLaunchOptions
        {
            Headless = IsHeadless
        };

        Browser = await _playwright.Chromium.LaunchAsync(options).ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();

        _playwright?.Dispose();
    }
}
