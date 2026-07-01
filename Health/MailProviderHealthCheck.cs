using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasket.Settings;

namespace SurveyBasket.Health;

public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings=mailSettings.Value;
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpClient = new SmtpClient();


            smtpClient.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

            smtpClient.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            smtpClient.Disconnect(true);
            return Task.FromResult(HealthCheckResult.Healthy("Mail provider is healthy."));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Mail provider is unhealthy.", ex));
        }
    }

}
