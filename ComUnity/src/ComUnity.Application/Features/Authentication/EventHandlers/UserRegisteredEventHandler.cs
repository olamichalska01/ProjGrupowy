using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Infrastructure.Services;
using ComUnity.Application.Infrastructure.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace ComUnity.Application.Features.Authentication.EventHandlers;

internal class UserRegisteredEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredEvent>>
{
    private readonly Links _links;
    private readonly ComUnityContext _context;
    private readonly IEmailSenderService _emailSender;
    private readonly string _fromEmail;

    public UserRegisteredEventHandler(ComUnityContext context, IEmailSenderService emailSender, IOptions<Links> links, IOptions<SmtpSettings> smtpSettings)
    {
        _links = links.Value;
        _context = context;
        _emailSender = emailSender;
        _fromEmail = smtpSettings.Value.Account;
    }

    public async Task Handle(DomainEventNotification<UserRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => x.Id == domainEvent.UserId, cancellationToken);

        if (user == null)
        {
            return;
        }

        var message = new MailMessage(_fromEmail, user.Email, "Email verification link", $"Use link below to verify your account. \n {_links.EmailVerificationLinkPrefix}{Uri.EscapeDataString(user.SecurityCode)}");
        await _emailSender.SendEmail(message);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
