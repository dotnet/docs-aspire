namespace SignalR.Web.Extensions;

public static class ConfigurationExtensions
{
    public static Uri GetServiceHttpUri(this IConfiguration config, string name) =>
        config.GetServiceUri(name, 0);

    public static Uri GetServiceHttpsUri(this IConfiguration config, string name) =>
        config.GetServiceUri(name, 1);

    private static Uri GetServiceUri(this IConfiguration config, string name, int index)
    {
        var url = config[$"services:{name}:{index}"];

        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return new(url);
    }
}
