using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MailDev.Client;

internal sealed class MailDevHealthCheck(Uri smtpUri) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new SmtpClient();

            await client.ConnectAsync(smtpUri, cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch
        {
            return HealthCheckResult.Unhealthy();
        }
    }
}
