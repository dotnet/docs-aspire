using MailKit.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MailDev.Client;

internal sealed class MailKitHealthCheck(MailKitClientFactory factory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // The factory connects (and authenticates).
            _ = await factory.GetSmtpClientAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
