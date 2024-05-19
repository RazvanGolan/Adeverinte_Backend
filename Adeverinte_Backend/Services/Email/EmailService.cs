using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Adeverinte_Backend.Services.Email;

public class EmailService : IEmailService
{
    private readonly string _from;
    private readonly string _smtpServer;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    
    public EmailService(IOptions<EmailConfig> configuration)
    {
        if (configuration == null || configuration.Value == null)
            throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        _from = configuration.Value.From ?? throw new ArgumentNullException(nameof(configuration.Value.From), "Sender email cannot be null.");
        _smtpServer = configuration.Value.EmailHost ?? throw new ArgumentNullException(nameof(configuration.Value.EmailHost), "SMTP server cannot be null.");
        _port = configuration.Value.Port;
        _username = configuration.Value.EmailUserName ?? throw new ArgumentNullException(nameof(configuration.Value.EmailUserName), "Email username cannot be null.");
        _password = configuration.Value.EmailPassword ?? throw new ArgumentNullException(nameof(configuration.Value.EmailPassword), "Email password cannot be null.");
    }
    public async Task SendEmailAsync(string emailReceiver, string emailTextBody, byte[] pdfBytes, string pdfName)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_from));
        email.To.Add(MailboxAddress.Parse(emailReceiver)); // pune emailReceiver
        email.Subject = "Adeverinta eliberata";

        var pdfAttachment = new MimePart
        {
            Content = new MimeContent(new MemoryStream(pdfBytes), ContentEncoding.Default),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = pdfName
        };
        
        if (string.IsNullOrEmpty(emailTextBody))
        {
            throw new ArgumentNullException(nameof(emailTextBody), "Email text body cannot be null or empty.");
        }
        
        var textPart = new TextPart("plain")
        {
            Text = emailTextBody
        };
        
        var multipart = new Multipart("mixed");
        multipart.Add(textPart);
        multipart.Add(pdfAttachment);
        
        email.Body = multipart;
        
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpServer, _port, SecureSocketOptions.SslOnConnect);
        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        await smtp.AuthenticateAsync(_username, _password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendEmailRejectedAsync(string emailReceiver, string emailTextBody)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_from));
        email.To.Add(MailboxAddress.Parse(emailReceiver)); // pune emailReceiver
        email.Subject = "Adeverinta respinsa";
        
        if (string.IsNullOrEmpty(emailTextBody))
        {
            throw new ArgumentNullException(nameof(emailTextBody), "Email text body cannot be null or empty.");
        }
        
        var textPart = new TextPart("plain")
        {
            Text = emailTextBody
        };
        
        var multipart = new Multipart("mixed");
        multipart.Add(textPart);
        
        email.Body = multipart;
        
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpServer, _port, SecureSocketOptions.SslOnConnect);
        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
        await smtp.AuthenticateAsync(_username, _password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}