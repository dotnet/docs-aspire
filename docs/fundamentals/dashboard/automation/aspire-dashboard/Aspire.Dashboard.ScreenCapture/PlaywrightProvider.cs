using Microsoft.Playwright;

namespace Aspire.Dashboard.ScreenCapture;

public class PlaywrightProvider
{
    public const string BrowserPathEnvironmentVariableName = "BROWSER_PATH";
    private const string PlaywrightBrowsersPathEnvironmentVariableName = "PLAYWRIGHT_BROWSERS_PATH";

    public static async Task<IBrowser> CreateBrowserAsync(BrowserTypeLaunchOptions? options = null)
    {
        var playwright = await Playwright.CreateAsync();
        string? browserPath = Environment.GetEnvironmentVariable(BrowserPathEnvironmentVariableName);
        if (!string.IsNullOrEmpty(browserPath) && !File.Exists(browserPath))
        {
            throw new FileNotFoundException($"Browser path {BrowserPathEnvironmentVariableName}='{browserPath}' does not exist");
        }

        options ??= new() { Headless = true };
        options.ExecutablePath ??= browserPath;

        if (OperatingSystem.IsMacOS() && string.IsNullOrEmpty(browserPath))
        {
            var probePath = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            if (File.Exists(probePath))
            {
                options.ExecutablePath = probePath;
            }
        }

        return await playwright.Chromium.LaunchAsync(options).ConfigureAwait(false);
    }

    public static void DetectAndSetInstalledPlaywrightDependenciesPath(DirectoryInfo? repoRoot = null)
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(PlaywrightBrowsersPathEnvironmentVariableName)))
        {
            // this would be the case for helix where the path is set to a
            // payload directory
            return;
        }

        repoRoot ??= FindRepoRoot();
        if (repoRoot is not null)
        {
            // Running from inside the repo
            // Check if we already have playwright-deps in artifacts
            var probePath = Path.Combine(repoRoot.FullName, "artifacts", "bin", "playwright-deps");
            if (Directory.Exists(probePath))
            {
                Environment.SetEnvironmentVariable(PlaywrightBrowsersPathEnvironmentVariableName, probePath);
                Console.WriteLine($"** Found playwright dependencies in {probePath}");
            }
            else
            {
                Console.WriteLine($"** Did not find playwright dependencies in {probePath}");
            }
        }
    }

    private static DirectoryInfo? FindRepoRoot()
    {
        DirectoryInfo? repoRoot = new(AppContext.BaseDirectory);

        while (repoRoot != null)
        {
            // To support git worktrees, check for either a directory or a file named ".git"
            if (Directory.Exists(Path.Combine(repoRoot.FullName, ".git")) || File.Exists(Path.Combine(repoRoot.FullName, ".git")))
            {
                return repoRoot;
            }

            repoRoot = repoRoot.Parent;
        }

        return null;
    }
}
