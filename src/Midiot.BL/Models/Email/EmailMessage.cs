namespace Midiot.BL.Models.Email;

public class EmailMessage
{
    public string RecipientEmail { get; init; }
    public Dictionary<string, string> TemplateData { get; init; }

    public EmailMessage(string recipientEmail, string name, string code)
    {
        RecipientEmail = recipientEmail;
        TemplateData = new Dictionary<string, string>
        {
            { "Name", name },
            { "Code", code }
        };
    }
}
