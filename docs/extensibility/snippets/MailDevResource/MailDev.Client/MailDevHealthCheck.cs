using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MailDev.Client;

internal sealed class MailDevHealthCheck(Uri smtpUri) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        SmtpClient client = new();

        try
        {
            await client.ConnectAsync(smtpUri, cancellationToken)
                .ConfigureAwait(false);

            return HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
