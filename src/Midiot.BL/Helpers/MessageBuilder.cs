using Midiot.BL.Models.Email;
using SendGrid.Helpers.Mail;

namespace Midiot.BL.Helpers;

public static class MessageBuilder
{
    public static EmailMessage BuildEmailMessage(string recipientEmail, string name, string code)
    {
        return new EmailMessage(recipientEmail, name, code);
    }

    public static SendGridMessage CreateSendGridMessage(EmailMessage message, string templateSendGridId)
    {
        var msg = new SendGridMessage
        {
            TemplateId = templateSendGridId
        };

        msg.AddTo(new EmailAddress(message.RecipientEmail));
        msg.SetTemplateData(message.TemplateData);

        return msg;
    }
}
