using System.Net;

namespace MailKit.Client;

/// <summary>
/// Provides the client configuration settings for connecting MailKit to an SMTP server.
/// </summary>
public sealed class MailKitClientSettings
{
    /// <summary>
    /// Gets or sets the SMTP server connection string. It should be represented in a
    /// valid <see cref="Uri"/> form.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the network credentials that are optionally configurable for SMTP
    /// server's that require authentication.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/>.
    /// </value>
    public NetworkCredential? Credentials { get; set; }

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
}
