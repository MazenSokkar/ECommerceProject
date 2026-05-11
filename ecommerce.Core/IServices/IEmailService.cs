namespace ecommerce.Core.IServices;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string template, Dictionary<string, string> templateModel, CancellationToken cancellationToken = default);
}
