namespace Adeverinte_Backend.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string emailSender, string emailTextBody, byte[] pdf, string pdfName);

    Task SendEmailRejectedAsync(string emailReceiver,string emailTextBody);
}