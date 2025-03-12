using SendGrid.Helpers.Mail;
using SendGrid;

namespace Midiot.BL.Interfaces.Email;

public interface IEmailService
{
    Task<Response> SendEmailAsync(SendGridMessage message);
}
