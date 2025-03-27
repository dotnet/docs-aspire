using Aspire.Dashboard.ScreenCapture.Extensions;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture;

[Trait("Category", "Automation")]
public class CaptureImages(AppHostTestFixture appHostTestFixture) : PlaywrightTestsBase<AppHostTestFixture>(appHostTestFixture)
{
    [Fact]
    public async Task CaptureHelpImages()
    {
        await ConfigureAsync<Projects.AspireSample_AppHost>();

        await RunTestAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.HelpButton);

            await page.EvaluateAsync($$"""
                document.querySelectorAll('{{DashboardSelectors.HelpButton}}').forEach(el => {
                  el.style.borderRadius = 0;
                  el.style.border = '3px solid #E80808';
                });
                """);

            // Take a screen capture of the dashboard
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/dashboard-help.png"
            });
        });
    }

    [Fact]
    public async Task CaptureLightAndDarkThemeImages()
    {
        await ConfigureAsync<Projects.AspireSample_AppHost>();

        await RunTestAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.SettingsButton);

            // Take a dark-theme screen capture.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/theme-selection.png"
            });

            // Change the theme to light theme.
            await page.ClickAsync(DashboardSelectors.LightThemeRadio);

            // Take a light-theme screen capture.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/theme-selection-light.png"
            });

            // Change theme back to dark theme.
            await page.ClickAsync(DashboardSelectors.DarkThemeRadio);
        });
    }

    [Fact]
    public async Task CaptureResourceTextVisualizerImages()
    {
        await ConfigureAsync<Projects.AspireSample_AppHost>();

        await RunTestAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);
        });
    }
}
