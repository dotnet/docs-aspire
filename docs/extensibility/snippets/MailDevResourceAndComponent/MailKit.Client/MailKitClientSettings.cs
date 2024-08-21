using System.Data.Common;

namespace MailKit.Client;

/// <summary>
/// Provides the client configuration settings for connecting MailKit to an SMTP server.
/// </summary>
public sealed class MailKitClientSettings
{
    internal const string DefaultConfigSectionName = "MailKit:Client";

    /// <summary>
    /// Gets or sets the SMTP server <see cref="Uri"/>.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the database health check is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry tracing is disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableTracing { get; set; }

    /// <summary>
    /// Gets or sets a boolean value that indicates whether the OpenTelemetry metrics are disabled or not.
    /// </summary>
    /// <value>
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool DisableMetrics { get; set; }

    internal void ParseConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"""
                    ConnectionString is missing.
                    It should be provided in 'ConnectionStrings:<connectionName>'
                    or '{DefaultConfigSectionName}:Endpoint' key.'
                    configuration section.
                    """);
        }

        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var uri))
        {
            Endpoint = uri;
        }
        else
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            
            if (builder.TryGetValue("Endpoint", out var endpoint) is false)
            {
                throw new InvalidOperationException($"""
                        The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                        '{DefaultConfigSectionName}') is missing.
                        """);
            }

            if (Uri.TryCreate(endpoint.ToString(), UriKind.Absolute, out uri) is false)
            {
                throw new InvalidOperationException($"""
                        The 'ConnectionStrings:<connectionName>' (or 'Endpoint' key in
                        '{DefaultConfigSectionName}') isn't a valid URI.
                        """);
            }

            Endpoint = uri;
        }
    }
}
