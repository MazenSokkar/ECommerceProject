using ecommerce.Core.IServices;
using ecommerce.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ecommerce.Infrastructure.Services;

public class EmailService(
    IOptions<MailSettings> mailSettings,
    IEmailBodyBuilder bodyBuilder,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string template, Dictionary<string, string> templateModel, CancellationToken cancellationToken = default)
    {
        try
        {
            var htmlBody = bodyBuilder.GenerateEmailBody(template, templateModel);

            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = subject,
            };

            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Recipient} with subject {Subject}", email, subject);
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}
