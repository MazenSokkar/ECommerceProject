using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Hosting;

namespace ecommerce.Infrastructure.Services;

public class EmailBodyBuilder(IWebHostEnvironment env) : IEmailBodyBuilder
{
    public string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
    {
        var templatePath = Path.Combine(env.ContentRootPath, "Templates", $"{template}.html");

        var body = File.ReadAllText(templatePath);

        foreach (var (key, value) in templateModel)
            body = body.Replace(key, value);

        return body;
    }
}
