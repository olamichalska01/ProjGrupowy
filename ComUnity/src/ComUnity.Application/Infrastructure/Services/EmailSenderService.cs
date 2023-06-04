using System.Net.Mail;
using System.Net;
using ComUnity.Application.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ComUnity.Application.Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSenderService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmail(MailMessage message)
    {
        var smtpClient = new SmtpClient(_smtpSettings.Host)
        {
            Port = _smtpSettings.Port,
            Credentials = new NetworkCredential(_smtpSettings.Account, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl,
        };

        await smtpClient.SendMailAsync(message);
    }
}

public interface IEmailSenderService
{
    Task SendEmail(MailMessage message);
}