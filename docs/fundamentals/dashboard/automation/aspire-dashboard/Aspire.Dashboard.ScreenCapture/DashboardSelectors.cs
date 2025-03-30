namespace Aspire.Dashboard.ScreenCapture;

internal static class DashboardSelectors
{
    public const string Toast = ".fluent-toast";

    public const string Dialog = "fluent-dialog";

    internal static class Nav
    {
        public const string Resources = $"""div.fluent-appbar-item[title="{nameof(Resources)}"] a""";

        public const string Console = $"""div.fluent-appbar-item[title="{nameof(Console)}"] a""";

        public const string Structured = $"""div.fluent-appbar-item[title="{nameof(Structured)}"] a""";

        public const string Traces = $"""div.fluent-appbar-item[title="{nameof(Traces)}"] a""";

        public const string Metrics = $"""div.fluent-appbar-item[title="{nameof(Metrics)}"] a""";
    }

    internal static class Header
    {
        public const string SettingsButton = """fluent-button[title="Settings"]""";

        public const string HelpButton = """fluent-button[title="Help"]""";
    }

    internal static class HelpDialog
    {
        public const string HelpDialogId = "#HelpDialog";
    }

    internal static class SettingsDialog
    {
        public const string SettingsDialogHeading = """#SettingsDialog h4""";

        public const string LightThemeRadio = """fluent-radio[current-value="Light"]""";

        public const string DarkThemeRadio = """fluent-radio[current-value="Dark"]""";
    }

    internal static class ResourcePage
    {
        public const string CacheResource = "cache";
        public const string ApiResource = "apiservice";
        public const string WebResource = "webfrontend";

        public const string ViewDetailsOption = """fluent-anchored-region fluent-menu-item:nth-child(1)""";

        public const string StopResource = """fluent-button[title="Stop resource"]""";
        public const string StartResource = """fluent-button[title="Start resource"]""";
        public const string SplitPanel = """fluent-button[title="Split horizontal"]""";

        public const string ResourceDetailsProjectPath = """fluent-accordion-item [title^="C:"]:last-of-type""";
    }
}
