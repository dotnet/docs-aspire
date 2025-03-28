using Aspire.Dashboard.ScreenCapture.Extensions;
using Microsoft.Playwright;
using Xunit;
using SampleAppHost = Projects.AspireSample_AppHost;

namespace Aspire.Dashboard.ScreenCapture;

[Trait("Category", "Automation")]
public class CaptureImages(AppHostTestFixture appHostTestFixture) : PlaywrightTestsBase<AppHostTestFixture>(appHostTestFixture)
{
    [Fact, Trait("Capture", "help-images")]
    public async Task CaptureHelpImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.Header.HelpButton);

            // Draw a red border (highlight) around element.
            await page.HighlightElementAsync(DashboardSelectors.Header.HelpButton);

            // Take a screen capture of the dashboard
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/dashboard-help.png"
            });
        });
    }

    [Fact, Trait("Capture", "project-resources")]
    public async Task CaptureProjectsAndResourceImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Take screen capture of the projects.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/projects.png"
            });

            // Hover over cache row
            await page.HoverAsync(FluentDataGridSelector.Grid.Body.Row(2));

            // Highlight stop "cache" button
            await page.HighlightElementAsync(DashboardSelectors.ResourcePage.StopResource);

            // Take screen capture of the projects.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/resource-stop-action.png"
            });
        },
        new() { Width = 1350, Height = 360 });
    }

    [Fact, Trait("Capture", "themes")]
    public async Task CaptureLightAndDarkThemeImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.Header.SettingsButton);

            // Wait for the settings dialog to be displayed.
            await page.WaitForSelectorAsync(DashboardSelectors.SettingsDialog.SettingsDialogHeading);

            // Take a dark-theme screen capture.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/theme-selection.png"
            });

            // Change the theme to light theme.
            await page.ClickAsync(DashboardSelectors.SettingsDialog.LightThemeRadio);

            // Take a light-theme screen capture.
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/theme-selection-light.png"
            });

            // Change theme back to dark theme.
            await page.ClickAsync(DashboardSelectors.SettingsDialog.DarkThemeRadio);
        },
        new() { Width = 1350, Height = 500 });
    }

    [Fact, Trait("Capture", "stop-start-resources")]
    public async Task CaptureResourceStopAndStartImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Select the stop "cache" button
            await page.ClickAsync(DashboardSelectors.ResourcePage.StopResource);

            // Wait for the notification to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.Toast);

            // Wait for the start button to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.ResourcePage.StartResource);

            // Highlight 'cache "Stop" succeeded' toast and 'Exited' state cell
            await page.HighlightElementsAsync(
                DashboardSelectors.Toast, FluentDataGridSelector.Grid.Body.Row(2).Cell(2));
                        
            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/resource-stopped-action.png"
            });
        },
        new() { Width = 1350, Height = 360 });

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            // Wait for the start button to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.ResourcePage.StartResource);

            // Select the start "cache" button
            await page.ClickAsync(DashboardSelectors.ResourcePage.StartResource);

            // Wait for the notification to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.Toast);

            var cacheStopButton = FluentDataGridSelector.Grid.Body.Row(2)
                .Descendant(DashboardSelectors.ResourcePage.StopResource);

            // Wait for the stop button to be displayed.
            await page.WaitForSelectorAsync(cacheStopButton);

            // Highlight 'cache "Start" succeeded' toast and 'Running' state cell.
            await page.HighlightElementsAsync(
                DashboardSelectors.Toast, FluentDataGridSelector.Grid.Body.Row(2).Cell(2));

            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/resource-started-action.png"
            });
        },
        new() { Width = 1350, Height = 360 });
    }

    [Fact, Trait("Capture", "resource-text-visualizer")]
    public async Task CaptureResourceTextVisualizerImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            var openInTextVisualizerButton = FluentDataGridSelector.Grid.Body.Row(2).Cell(5)
                .Descendant("fluent-button");

            // Hover over "open in text visualizer"
            await page.HoverAsync(openInTextVisualizerButton);

            // Highlight the button
            await page.HighlightElementAsync(openInTextVisualizerButton);

            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/text-visualizer-selection-menu.png"
            });

            // Click "open in text visualizer"
            await page.ClickAsync(openInTextVisualizerButton);

            // Wait for dialog to be attached.
            await page.WaitForSelectorAsync(DashboardSelectors.Dialog, new() { State = WaitForSelectorState.Attached });

            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/text-visualizer-resources.png"
            });
        },
        new() { Width = 1350, Height = 360 });
    }

    [Fact, Trait("Capture", "resource-details")]
    public async Task CaptureResourceDetailImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForCacheResourceAsync(DashboardLoginToken);

            var apiEllipsisButton = FluentDataGridSelector.Grid.Body.Row(3).Cell(7)
                .Descendant("fluent-button:nth-of-type(3)");
            await page.ClickAsync(apiEllipsisButton);

            await page.ClickAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.ClickAsync(DashboardSelectors.ResourcePage.SplitPanel);

            await page.ClickAndDragShadowRootElementAsync("split-panels", "#median", (0, 20));
            await page.RedactElementTextAsync("""fluent-accordion-item [title^="C:"]:last-of-type""");

            await page.ClickAsync(apiEllipsisButton);
            await page.HoverAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.HighlightElementAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);

            await page.ScreenshotAsync(new()
            {
                Path = "../../../../../../media/explore/resource-details.png"
            });
        });
    }
}
