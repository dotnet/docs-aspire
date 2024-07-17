using MailKit.Net.Smtp;

namespace MailKit.Client;

/// <summary>
/// A factory for creating <see cref="ISmtpClient"/> instances
/// given a <paramref name="smtpUri"/> (and optional <paramref name="credentials"/>).
/// </summary>
/// <param name="settings">
/// The <see cref="MailKitClientSettings"/> settings for the SMTP server
/// </param>
public sealed class MailKitClientFactory(MailKitClientSettings settings) : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private SmtpClient? _client;

    /// <summary>
    /// Gets an <see cref="ISmtpClient"/> instance in the connected state
    /// (and that's been authenticated if configured).
    /// </summary>
    /// <param name="cancellationToken">Used to abort client creation and connection.</param>
    /// <returns>A connected (and authenticated) <see cref="ISmtpClient"/> instance.</returns>
    /// <remarks>
    /// Since both the connection and authentication are considered expensive operations,
    /// the <see cref="ISmtpClient"/> returned is intended to be used for the duration of a request
    /// (registered as 'Scoped') and is automatically disposed of.
    /// </remarks>
    public async Task<ISmtpClient> GetSmtpClientAsync(
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_client is null)
            {
                _client = new SmtpClient();

                await _client.ConnectAsync(settings.Endpoint, cancellationToken)
                             .ConfigureAwait(false);
            }
        }
        finally
        {
            _semaphore.Release();
        }       

        return _client;
    }

    public void Dispose()
    {
        _client?.Dispose();
        _semaphore.Dispose();
    }
}
