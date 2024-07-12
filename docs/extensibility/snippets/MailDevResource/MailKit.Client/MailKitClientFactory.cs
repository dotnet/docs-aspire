using System.Net;
using MailKit.Net.Smtp;

namespace MailKit.Client;

/// <summary>
/// A factory for creating <see cref="ISmtpClient"/> instances
/// given a <paramref name="smtpUri"/> (and optional <paramref name="credentials"/>).
/// </summary>
/// <param name="smtpUri">The <see cref="Uri"/> for the SMTP server</param>
/// <param name="credentials">The optional <see cref="ICredentials"/> used to authenticate to the SMTP server</param>
public sealed class MailKitClientFactory(Uri smtpUri, ICredentials? credentials = null)
{
    /// <summary>
    /// Gets an <see cref="ISmtpClient"/> instance in the connected state
    /// (and that's been authenticated if configured).
    /// </summary>
    /// <param name="cancellationToken">Used to abort client creation and connection.</param>
    /// <returns>A connected (and authenticated) <see cref="ISmtpClient"/> instance.</returns>
    /// <remarks>
    /// Since both the connection and authentication are considered expensive operations,
    /// the <see cref="ISmtpClient"/> returned is intended to be used for the duration of a request
    /// (registered as 'Scoped') and shouldn't be disposed of and disconnect shouldn't be called.
    /// </remarks>
    public async Task<ISmtpClient> GetSmtpClientAsync(
        CancellationToken cancellationToken = default)
    {
        SmtpClient client = new();

        await client.ConnectAsync(smtpUri, cancellationToken)
                    .ConfigureAwait(false);

        if (credentials is not null)
        {
            await client.AuthenticateAsync(credentials, cancellationToken)
                        .ConfigureAwait(false);
        }

        return client;
    }
}
