using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace MailDev.Client;

/// <summary>
/// Provides extension methods for registering a <see cref="MailDevClient"/> as a
/// scoped-lifetime service in the services provided by the <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class MailDevClientServiceCollectionExtensions
{
    private const string DefaultConfigSectionName = "Aspire:MailDev:Client";

    /// <summary>
    /// Registers 'Scoped' <see cref="MailDevClient" /> for sending emails.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="connectionName">A name used to retrieve the connection string from the ConnectionStrings configuration section.</param>
    /// <param name="configureSettings">An optional delegate that can be used for customizing options. It's invoked after the settings are read from the configuration.</param>
    public static void AddMailDevClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<MailDevClientSettings>? configureSettings = null) =>
        AddMailDevClient(
            builder,
            DefaultConfigSectionName,
            configureSettings,
            connectionName,
            serviceKey: null);

    /// <summary>
    /// Registers 'Scoped' <see cref="MailDevClient" /> for sending emails.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="name">The name of the component, which is used as the <see cref="ServiceDescriptor.ServiceKey"/> of the service and also to retrieve the connection string from the ConnectionStrings configuration section.</param>
    /// <param name="configureSettings">An optional method that can be used for customizing options. It's invoked after the settings are read from the configuration.</param>
    public static void AddKeyedMailDevClient(
        this IHostApplicationBuilder builder,
        string name,
        Action<MailDevClientSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        AddMailDevClient(
            builder,
            $"{DefaultConfigSectionName}:{name}",
            configureSettings,
            connectionName: name,
            serviceKey: name);
    }

    private static void AddMailDevClient(
        this IHostApplicationBuilder builder,
        string configurationSectionName,
        Action<MailDevClientSettings>? configureSettings,
        string connectionName,
        object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new MailDevClientSettings();

        builder.Configuration
               .GetSection(configurationSectionName)
               .Bind(settings);

        configureSettings?.Invoke(settings);

        var smtpUri = GetMailDevSmtpUri(builder.Configuration, connectionName);

        if (serviceKey is null)
        {
            builder.Services.AddScoped(_ => new MailDevClient(smtpUri));
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (_, __) => new MailDevClient(smtpUri));
        }

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .Add(new HealthCheckRegistration(
                    "Aspire.HealthChecks.MailDev",
                    _ => new MailDevHealthCheck(smtpUri),
                    failureStatus: default,
                    tags: default));
        }
    }

    private static Uri GetMailDevSmtpUri(IConfigurationManager config, string name)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (config.GetConnectionString(name) is not { } connectionString)
        {
            throw new ArgumentException(
                $"Missing connection string for {name}", nameof(name));
        }

        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var smtpUri) is false ||
            smtpUri is null)
        {
            throw new ArgumentException(
                $"Connection string isn't a URI for {name}", nameof(name));
        }

        return smtpUri;
    }
}
