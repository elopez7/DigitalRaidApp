using DigitalRaid.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace DigitalRaid.Services;

public class DREmailService : IEmailSender
{
    private readonly MailSettings _mailSettings;

    public DREmailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(string emailDestination, string subject, string htmlMessage)
    {
        var emailSender = _mailSettings.Email ?? Environment.GetEnvironmentVariable("Email");
        MimeMessage emailMessage = new();

        emailMessage.Sender =MailboxAddress.Parse(emailSender);

        foreach(var emailAddress in emailDestination.Split(";"))
        {
            emailMessage.To.Add(MailboxAddress.Parse(emailAddress));
        }

        emailMessage.Subject = subject;


        BodyBuilder emailBody = new()
        { 
            HtmlBody = htmlMessage
        };

        emailMessage.Body = emailBody.ToMessageBody();

        using SmtpClient smtpClient = new();

        try
        {
            var host = _mailSettings.Host ?? Environment.GetEnvironmentVariable("Host");
            var port = _mailSettings.Port != 0 ? _mailSettings.Port : int.Parse(Environment.GetEnvironmentVariable("Port"));
            var password = _mailSettings.Password ?? Environment.GetEnvironmentVariable("Password");

            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, password);

            await smtpClient.SendAsync(emailMessage);
            await smtpClient.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            var error = ex.Message;
            throw;
        }

    }
}
