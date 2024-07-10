
using Microsoft.Extensions.Hosting;

namespace MailDev.Client;

public static class MailDevClientServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMailDevClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<MailDevClientSettings>? configure)
    {


        return builder;
    }
}
