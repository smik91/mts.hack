using Microsoft.Extensions.Options;
using Midiot.BL.Interfaces.Email;
using Midiot.Common.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Midiot.BL.Services.Email;

public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly string _fromEmail;

    public EmailService(IOptions<SendGridOptions> options)
    {
        _client = new SendGridClient(options.Value.ApiKey);
        _fromEmail = options.Value.FromEmail;
    }

    public async Task<Response> SendEmailAsync(SendGridMessage message)
    {
        message.From = new EmailAddress(_fromEmail);
        var response = await _client.SendEmailAsync(message);
        return response;
    }
}
