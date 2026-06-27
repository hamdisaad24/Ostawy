using Microsoft.Extensions.Options;
using Ostawy.Helpers;
using Ostawy.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Ostawy.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task SendEmailAsync(string receptor, string Sub, string body)
    {
        var host = _emailSettings.Host;
        var port = _emailSettings.Port;
        var emailUser = _emailSettings.Email;
        var password = _emailSettings.Password;

        var smtpClient = new SmtpClient(host, int.Parse(port.ToString()));
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;

        smtpClient.Credentials = new NetworkCredential(emailUser, password);
        string Subject = Sub;
        string Body = body;
        var message = new MailMessage(emailUser ?? "error@fmf.com", receptor, Subject, Body)
        {
            IsBodyHtml = true
        };
        await smtpClient.SendMailAsync(message);
    }
}
