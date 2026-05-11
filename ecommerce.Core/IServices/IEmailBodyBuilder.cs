namespace ecommerce.Core.IServices;

public interface IEmailBodyBuilder
{
    string GenerateEmailBody(string template, Dictionary<string, string> templateModel);
}
