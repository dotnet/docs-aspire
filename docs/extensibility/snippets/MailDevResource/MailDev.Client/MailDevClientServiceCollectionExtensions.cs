using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace MailDev.Client;

public static class MailDevClientServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddMailDevClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<MailDevClientSettings>? configureSettings = default)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new MailDevClientSettings();

        builder.Configuration
               .GetSection("Aspire:MailDev")
               .Bind(settings);

        configureSettings?.Invoke(settings);

        var smtpUri = TryGetSmtpUri(builder.Configuration, connectionName);

        builder.Services.AddScoped(_ =>
        {
            return new MailDevClient(smtpUri);
        });

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .Add(new HealthCheckRegistration(
                    "Aspire.HealthChecks.MailDev",
                    _ => new MailDevHealthCheck(smtpUri),
                    failureStatus: default,
                    tags: default));
        }

        return builder; 
    }

    private static Uri TryGetSmtpUri(IConfigurationManager config, string name)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (config.GetConnectionString(name) is not { } connectionString)
        {
            throw new ArgumentException(
                $"Missing connection string for {name}", nameof(name));
        }

        if (Uri.TryCreate(connectionString, UriKind.RelativeOrAbsolute, out var smtpUri) ||
            smtpUri is null)
        {
            throw new ArgumentException(
                $"Connection string isn't a URI for {name}", nameof(name));
        }

        return smtpUri;
    }
}
