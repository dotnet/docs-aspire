namespace SignalR.Web.Extensions;

public static class ConfigurationExtensions
{
    public static Uri GetServiceHttpUri(this IConfiguration config, string name) =>
        config.GetServiceUri(name, "http", 0);

    public static Uri GetServiceHttpsUri(this IConfiguration config, string name) =>
        config.GetServiceUri(name, "https", 0);

    private static Uri GetServiceUri(this IConfiguration config, string name, string scheme, int index)
    {
        var url = config[$"services:{name}:{scheme}:{index}"];

        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return new(url);
    }
}
