using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MailDev.Client;

/// <summary>
/// A representation of a MailDev client.
/// </summary>
public sealed class MailDevClient : IDisposable
{
    private readonly string _from;
    private readonly SmtpClient _smtpClient;

    /// <summary>
    /// Initializes a new instance of <see cref="MailDevClient"/> with the
    /// given <paramref name="configuration"/> and <paramref name="name"/>.
    /// </summary>
    /// <param name="configuration">
    /// The configuration containing the named (<paramref name="name"/>) connection string for the SMTP server.
    /// </param>
    /// <param name="name">
    /// The name used to resolve the connection string
    /// </param>
    public MailDevClient(IConfiguration configuration, string name)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _from = configuration["MailDev:NewsletterEmail"]
            ?? "newsletter@yourcompany.com";

        var connectionString = configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                $"A connection string is required for {name}");
        }

        if (Uri.TryCreate(connectionString, UriKind.Absolute, out var smtpUri) is false)
        {
            throw new ArgumentException(
                $"The configured connection string for {name} isn't a valid URI: {connectionString}");
        }

        _smtpClient = new SmtpClient(smtpUri.Host, smtpUri.Port);
    }

    /// <summary>
    /// Subscribes the given <paramref name="email"/> to the newsletter.
    /// </summary>
    /// <param name="email">The email address that's subscribing.</param>
    /// <returns>
    /// An asynchronous operation representing the subscribe functionality.
    /// </returns>
    public async Task SubscribeToNewsletterAsync(string email)
    {
        using var message = new MailMessage(_from, email)
        {
            Subject = "Welcome to our newsletter!",
            Body = "Thank you for subscribing to our newsletter!"
        };

        await _smtpClient.SendMailAsync(message);
    }

    /// <summary>
    /// Unsubscribes the given <paramref name="email"/> to the newsletter.
    /// </summary>
    /// <param name="email">The email address that's unsubscribing.</param>
    /// <returns>
    /// An asynchronous operation representing the unsubscribe functionality.
    /// </returns>
    public async Task UnsubscribeToNewsletterAsync(string email)
    {
        using var message = new MailMessage(_from, email)
        {
            Subject = "You are unsubscribed from our newsletter!",
            Body = "Sorry to see you go. We hope you will come back soon!"
        };

        await _smtpClient.SendMailAsync(message);
    }

    /// <inheritdoc cref="SmtpClient.Dispose()" />
    public void Dispose() => _smtpClient.Dispose();
}
