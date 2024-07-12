using MailKit.Net.Smtp;
using MimeKit;

namespace MailDev.Client;

/// <summary>
/// A representation of a MailDev client.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="MailDevClient"/> with the
/// given <paramref name="smtpUri"/>.
/// </remarks>
/// <param name="smtpUri">
/// The <see cref="Uri"/> for the SMTP server.
/// </param>
public sealed class MailDevClient(Uri smtpUri)
{
    private readonly string _from = "newsletter@yourcompany.com";

    /// <summary>
    /// Subscribes the given <paramref name="email"/> to the newsletter.
    /// </summary>
    /// <param name="email">The email address that's subscribing.</param>
    /// <returns>
    /// An asynchronous operation representing the subscribe functionality.
    /// </returns>
    public Task SubscribeToNewsletterAsync(string email)
    {
        var message = new MimeMessage
        {
            Subject = "Welcome to our newsletter!",
            Body = new TextPart("plain")
            {
                Text = "Thank you for subscribing to our newsletter!"
            },
            From = { new MailboxAddress("Dev Newsletter", _from) },
            To = { new MailboxAddress("Recipient Name", email) }
        };

        return SendMessageAsync(message);
    }

    /// <summary>
    /// Unsubscribes the given <paramref name="email"/> to the newsletter.
    /// </summary>
    /// <param name="email">The email address that's unsubscribing.</param>
    /// <returns>
    /// An asynchronous operation representing the unsubscribe functionality.
    /// </returns>
    public Task UnsubscribeToNewsletterAsync(string email)
    {
        var message = new MimeMessage
        {
            Subject = "You are unsubscribed from our newsletter!",
            Body = new TextPart("plain")
            {
                Text = "Sorry to see you go. We hope you will come back soon!"
            },
            From = { new MailboxAddress("Dev Newsletter", _from) },
            To = { new MailboxAddress("Recipient Name", email) }
        };

        return SendMessageAsync(message);
    }

    private async Task SendMessageAsync(MimeMessage message)
    {
        using var client = new SmtpClient();

        await client.ConnectAsync(smtpUri.Host, smtpUri.Port);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
