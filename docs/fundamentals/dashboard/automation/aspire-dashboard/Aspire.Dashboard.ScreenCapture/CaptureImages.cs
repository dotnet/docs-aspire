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
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.Header.HelpButton);

            // Draw a red border (highlight) around element.
            await page.HighlightElementAsync(DashboardSelectors.Header.HelpButton);

            await page.SaveExploreScreenshotAsync("dashboard-help.png");
        });
    }

    [Fact, Trait("Capture", "project-resources")]
    public async Task CaptureProjectsAndResourceImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Take screen capture of the projects.
            await page.SaveExploreScreenshotAsync("projects.png");

            // Hover over cache row
            await page.HoverAsync(FluentDataGridSelector.Grid.Body.Row(2));

            // Highlight stop "cache" button
            await page.HighlightElementAsync(DashboardSelectors.ResourcePage.StopResource);

            // Take screen capture of the projects.
            await page.SaveExploreScreenshotAsync("resource-stop-action.png");
        },
        new() { Width = 1280, Height = 400 });

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Click the "graph" tab
            await page.ClickAsync(DashboardSelectors.ResourcePage.TabGraph);

            // Wait for animation to finish
            await Task.Delay(1_000);

            // Wait for the graph to be displayed
            await page.SaveExploreScreenshotAsync("project-graphs.png");
        });
    }

    [Fact, Trait("Capture", "themes")]
    public async Task CaptureLightAndDarkThemeImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Click the settings cog.
            await page.ClickAsync(DashboardSelectors.Header.SettingsButton);

            // Wait for the settings dialog to be displayed.
            await page.WaitForSettingsFlyoutAsync();

            // Take a dark-theme screen capture.
            await page.SaveExploreScreenshotAsync("theme-selection.png");

            // Change the theme to light theme.
            await page.ClickAsync(DashboardSelectors.SettingsDialog.LightThemeRadio);

            // Take a light-theme screen capture.
            await page.SaveExploreScreenshotAsync("theme-selection-light.png");

            // Change theme back to dark theme.
            await page.ClickAsync(DashboardSelectors.SettingsDialog.DarkThemeRadio);
        },
        new() { Width = 1280, Height = 500 });
    }

    [Fact, Trait("Capture", "stop-start-resources")]
    public async Task CaptureResourceStopAndStartImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Select the stop "cache" button
            await page.ClickAsync(DashboardSelectors.ResourcePage.StopResource);

            // Wait for the notification to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.Toast);

            // Wait for the start button to be displayed
            await page.WaitForSelectorAsync(DashboardSelectors.ResourcePage.StartResource);

            // // Highlight 'cache "Stop" succeeded' toast and 'Exited' state cell
            // await page.HighlightElementsAsync(
            //     DashboardSelectors.Toast, FluentDataGridSelector.Grid.Body.Row(2).Cell(2));

            await page.SaveExploreScreenshotAsync("resource-stopped-action.png");
        },
        new() { Width = 1280, Height = 400 });

        await InteractWithPageAsync(async page =>
        {
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken, false);

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

            // // Highlight 'cache "Start" succeeded' toast and 'Running' state cell.
            // await page.HighlightElementsAsync(
            //     DashboardSelectors.Toast, FluentDataGridSelector.Grid.Body.Row(2).Cell(2));

            await page.SaveExploreScreenshotAsync("resource-started-action.png");
        },
        new() { Width = 1280, Height = 400 });
    }

    [Fact, Trait("Capture", "resource-text-visualizer")]
    public async Task CaptureResourceTextVisualizerImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            var openInTextVisualizerButton = FluentDataGridSelector.Grid.Body.Row(2).Cell(4)
                .Descendant("fluent-button");

            // Hover over "open in text visualizer"
            await page.HoverAsync(openInTextVisualizerButton);

            // Highlight the button
            await page.HighlightElementAsync(openInTextVisualizerButton);

            await page.SaveExploreScreenshotAsync("text-visualizer-selection-menu.png");

            // Click "open in text visualizer"
            await page.ClickAsync(openInTextVisualizerButton);

            // Wait for dialog to be attached.
            await page.WaitForSelectorAsync(DashboardSelectors.Dialog, new() { State = WaitForSelectorState.Attached });

            await page.SaveExploreScreenshotAsync("text-visualizer-resources.png");
        },
        new() { Width = 1280, Height = 400 });
    }

    [Fact, Trait("Capture", "resource-details")]
    public async Task CaptureResourceDetailImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            var apiEllipsisButton = FluentDataGridSelector.Grid.Body.Row(3).Cell(6)
                .Descendant("fluent-button:nth-of-type(3)");
            await page.ClickAsync(apiEllipsisButton);

            await page.HighlightElementAsync("fluent-anchored-region");

            await page.SaveExploreScreenshotAsync("resource-actions.png");

            await page.ClickAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.ClickAsync(DashboardSelectors.ResourcePage.SplitPanel);

            await page.AdjustSplitPanelsGridTemplateAsync();
            await page.ClickAndDragShadowRootElementAsync(
                DashboardSelectors.SplitPanels, DashboardSelectors.MedianId, (0, 20));
            await page.RedactElementTextAsync(DashboardSelectors.ResourcePage.ResourceDetailsProjectPath);

            await page.ClickAsync(apiEllipsisButton);
            await page.HoverAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.HighlightElementAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);

            await page.SaveExploreScreenshotAsync("resource-details.png");
        },
        new() { Width = 1280, Height = 800 });
    }

    [Fact, Trait("Capture", "resource-filtering")]
    public async Task CaptureResourceFilteringImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            await page.ClickAsync(DashboardSelectors.ResourcePage.FilterButton);

            await page.HighlightElementAsync(DashboardSelectors.ResourcePage.FilterDiv);

            await page.SaveExploreScreenshotAsync("select-resource-type.png");
        },
        new() { Width = 1280, Height = 550 });

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            await page.ClickAsync(DashboardSelectors.ResourcePage.FilterButton);
            await page.ClickAsync("#resource-types > fluent-checkbox:nth-of-type(1)");
            await page.ClickAsync("#resource-types > fluent-checkbox:nth-of-type(2)");
            await page.ClickAsync(DashboardSelectors.TopHeader, new() { Force = true });

            var cacheEllipsisButton = FluentDataGridSelector.Grid.Body.Row(2).Cell(6)
                .Descendant("fluent-button:nth-of-type(3)");
            await page.ClickAsync(cacheEllipsisButton);

            await page.ClickAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.ClickAsync(DashboardSelectors.ResourcePage.SplitPanel);

            await page.AdjustSplitPanelsGridTemplateAsync();

            await page.ClickAsync(DashboardSelectors.ResourcePage.FilterButton);

            await page.SaveExploreScreenshotAsync("resources-filtered-containers.png");
        },
        new() { Width = 1280, Height = 630 });
    }

    [Fact, Trait("Capture", "resource-errors")]
    public async Task CaptureResourcesWithErrorsImages()
    {
        await ConfigureAsync<SampleAppHost>([ "API_THROWS_EXCEPTION=true" ]);

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Get the weather web frontend URL
            var url = await page.GetResourceEndpointAsync();
            var webPage = await page.Context.NewPageAsync();

            await webPage.GotoAsync($"{url}/weather", new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await Task.Delay(7_500);

            await page.BringToFrontAsync();
            await page.SaveExploreScreenshotAsync("projects-errors.png");

            // Click the api "errors" button
            var apiErrorButton = FluentDataGridSelector.Grid.Body.Row(4).Cell(2)
                .Descendant("> div > fluent-anchor a");
            await page.ClickAsync(apiErrorButton);

            // Click the first action button
            var firstActionButton = FluentDataGridSelector.Grid.Body.Row(3).Cell(6)
                .Descendant("fluent-button");
            await page.ClickAsync(firstActionButton);

            await Task.Delay(1_500);

            await page.HighlightElementAsync("fluent-anchored-region");

            await page.SaveExploreScreenshotAsync("structured-logs-errors.png");
        },
        new() { Width = 1280, Height = 460 });
    }

    [Fact, Trait("Capture", "structured-logs-errors")]
    public async Task CaptureStructuredLogsErrorsImages()
    {
        await ConfigureAsync<SampleAppHost>(["API_THROWS_EXCEPTION=true"]);

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Get the weather web frontend URL
            var url = await page.GetResourceEndpointAsync();
            var webPage = await page.Context.NewPageAsync();

            await webPage.GotoAsync($"{url}/weather", new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await Task.Delay(1_500);

            await page.BringToFrontAsync();

            var apiConsoleLogs = FluentDataGridSelector.Grid.Body.Row(3)//.Cell(7)
                .Descendant("""fluent-button[title="Console logs"]""");
            await page.ClickAsync(apiConsoleLogs);

            await Task.Delay(1000);

            await page.SaveExploreScreenshotAsync("project-logs-error.png");

            await page.ClickAsync(DashboardSelectors.Nav.Resources);

            // Click the api "errors" button
            var apiErrorButton = FluentDataGridSelector.Grid.Body.Row(4).Cell(2)
                .Descendant("> div > fluent-anchor a");
            await page.ClickAsync(apiErrorButton);

            // Click the first actions button
            var firstActionButton = FluentDataGridSelector.Grid.Body.Row(2).Cell(6)
                .Descendant("fluent-button");
            await page.ClickAsync(firstActionButton);

            await page.ClickAsync(DashboardSelectors.ResourcePage.ViewDetailsOption);
            await page.ClickAsync(DashboardSelectors.ResourcePage.SplitPanel);

            await page.AdjustSplitPanelsGridTemplateAsync();

            await page.SaveExploreScreenshotAsync("structured-logs-errors-view.png");
        },
        new() { Width = 1280, Height = 960 });
    }

    [Fact, Trait("Capture", "console-logs")]
    public async Task CaptureConsoleLogsOutputImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            var apiConsoleLogs = FluentDataGridSelector.Grid.Body.Row(3)//.Cell(7)
                .Descendant("""fluent-button[title="Console logs"]""");
            await page.ClickAsync(apiConsoleLogs);

            await Task.Delay(1_000);

            await page.RedactElementTextAsync("#logContainer > div:nth-child(11) > div > span > span.log-content");

            await page.SaveExploreScreenshotAsync("project-logs.png");

            // Select "cache" dropdown
            await page.ClickAsync("fluent-select.resource-list");

            await page.ClickAsync("""fluent-option[value="cache"]""");

            await Task.Delay(1_000);

            await page.SaveExploreScreenshotAsync("container-logs.png");
        },
        new() { Width = 1280, Height = 400 });
    }

    [Fact, Trait("Capture", "structured-logs")]
    public async Task CaptureStructuredLogsImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Get the weather web frontend URL
            var url = await page.GetResourceEndpointAsync();
            var webPage = await page.Context.NewPageAsync();

            await webPage.GotoAsync($"{url}/weather", new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await Task.Delay(1_500);

            await page.BringToFrontAsync();

            await page.ClickAsync(DashboardSelectors.Nav.Structured);

            await Task.Delay(1000);

            await page.SaveExploreScreenshotAsync("structured-logs.png");

            // Click the add filters button
            await page.ClickAsync("""fluent-button[aria-label="Add filter"]""");

            await page.ApplyInlineStyleAsync("form",
                ("border", "3px solid red"), ("padding", "0.2rem 1rem"));

            await page.SaveExploreScreenshotAsync("structured-logs-filtered.png");
        });
    }

    [Fact, Trait("Capture", "trace-logs")]
    public async Task CaptureTraceLogsImages()
    {
        await ConfigureAsync<SampleAppHost>();

        await InteractWithPageAsync(async page =>
        {
            // Login to the dashboard
            await page.LoginAndWaitForRunningResourcesAsync(DashboardLoginToken);

            // Get the weather web frontend URL
            var url = await page.GetResourceEndpointAsync();
            var webPage = await page.Context.NewPageAsync();

            await webPage.GotoAsync($"{url}/weather", new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            // Reload to force output cache
            await webPage.ReloadAsync();
            await webPage.ReloadAsync();

            // Delay beyond output cache, and invalidate then reload.
            await Task.Delay(2_250);
            await webPage.ReloadAsync();

            await page.BringToFrontAsync();

            await page.ClickAsync(DashboardSelectors.Nav.Traces);

            await Task.Delay(1000);

            await page.SaveExploreScreenshotAsync("traces.png");

            var filterInput = """[placeholder="Filter..."]""";
            var filter = page.Locator($"{filterInput} #control");
            await filter.FillAsync("weather");
            await page.HighlightElementAsync(filterInput);

            await Task.Delay(250);

            await page.SaveExploreScreenshotAsync("trace-view-filter.png");
        });
    }
}
