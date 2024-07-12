using MailDev.Client;
using MailKit;
using MailKit.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Provides extension methods for registering a <see cref="SmtpClient"/> as a
/// scoped-lifetime service in the services provided by the <see cref="IHostApplicationBuilder"/>.
/// </summary>
public static class MailKitClientServiceCollectionExtensions
{
    private const string DefaultConfigSectionName = "Aspire:MailKit:Client";

    /// <summary>
    /// Registers 'Scoped' <see cref="MailDevClient" /> for sending emails.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="connectionName">A name used to retrieve the connection string from the ConnectionStrings configuration section.</param>
    /// <param name="configureSettings">An optional delegate that can be used for customizing options. It's invoked after the settings are read from the configuration.</param>
    public static void AddMailDevClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<MailKitClientSettings>? configureSettings = null) =>
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
        Action<MailKitClientSettings>? configureSettings = null)
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
        Action<MailKitClientSettings>? configureSettings,
        string connectionName,
        object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var settings = new MailKitClientSettings();

        builder.Configuration
               .GetSection(configurationSectionName)
               .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            settings.ConnectionString = connectionString;
        }

        configureSettings?.Invoke(settings);

        MailKitClientFactory CreateMailKitClientFactory(IServiceProvider sp)
        {
            var connectionString = settings.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"""
                    ConnectionString is missing.
                    It should be provided in 'ConnectionStrings:{connectionName}'
                    or under 'ConnectionString' key in '{DefaultConfigSectionName}'
                    configuration section.
                    """);
            }

            if (Uri.TryCreate(connectionString, UriKind.Absolute, out var smtpUri) is false ||
                smtpUri is null)
            {
                throw new InvalidOperationException($"""
                    The 'ConnectionStrings:{connectionName}' (or 'ConnectionString' key in
                    '{DefaultConfigSectionName}') isn't a valid URI format.
                    """);
            }

            return new MailKitClientFactory(smtpUri, settings.Credentials);
        }

        if (serviceKey is null)
        {
            builder.Services.AddScoped(CreateMailKitClientFactory);
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (sp, _) => CreateMailKitClientFactory(sp));
        }

        if (settings.DisableHealthChecks is false)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<MailKitHealthCheck>(
                    name: serviceKey is null ? "MailKit" : $"MailKit_{connectionName}",
                    failureStatus: default,
                    tags: [ "live" ]);
        }

        if (settings.DisableTracing is false)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(
                    traceBuilder => traceBuilder.AddSource(
                        Telemetry.SmtpClient.ActivitySourceName));
        }

        if (settings.DisableMetrics is false)
        {
            builder.Services.AddOpenTelemetry()
                .WithMetrics(
                    metricsBuilder => metricsBuilder.AddMeter(
                        Telemetry.SmtpClient.MeterName));
        }
    }
}
