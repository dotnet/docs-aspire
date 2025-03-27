namespace Aspire.Dashboard.ScreenCapture;

internal static class DashboardSelectors
{
    public const string SettingsButton = """fluent-button[title="Settings"]""";

    public const string HelpButton = """fluent-button[title="Help"]""";

    public const string SettingsDialogHeading = """#SettingsDialog h4""";

    public const string LightThemeRadio = """fluent-radio[current-value="Light"]""";

    public const string DarkThemeRadio = """fluent-radio[current-value="Dark"]""";
}

internal static class ResourcePageTitles
{
    public const string CacheResource = "cache";
    public const string ApiResource = "apiservice";
    public const string WebResource = "webfrontend";
}
