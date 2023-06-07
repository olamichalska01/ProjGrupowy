using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Features.Authentication.Utils;
using ComUnity.Application.Infrastructure.Services;
using ComUnity.Application.Infrastructure.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace ComUnity.Application.Features.Authentication;

public class ResetPasswordController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPut("/api/auth/reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

public record ResetPasswordCommand(string Email) : IRequest<Unit>;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private const int PasswordResetVerificationCodeLength = 64;
    private const int PasswordResetVerificationCodeValidityMinutes= 30;
    private readonly ComUnityContext _context;
    private readonly string _passwordResetLink;
    private readonly IEmailSenderService _emailService;

    public ResetPasswordCommandHandler(ComUnityContext context, IEmailSenderService emailService, IOptions<Links> links)
    {
        _context = context;
        _emailService = emailService;
        _passwordResetLink = links.Value.PasswordResetLink;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user == null)
        {
            return Unit.Value;
        }

        var securityCode = RandomSecurityStringGenerator.Generate(PasswordResetVerificationCodeLength);
        var codeExpirationDate = DateTime.UtcNow.AddMinutes(PasswordResetVerificationCodeValidityMinutes);

        user.SetPasswordResetSecurityCode(securityCode, codeExpirationDate);
        await _context.SaveChangesAsync(cancellationToken);

        var message = new MailMessage(_emailService.From, user.Email, "Password reset requested",
            $"Use link below to reset your password \n {_passwordResetLink}{securityCode}");

        await _emailService.SendEmail(message);

        return Unit.Value;
    }
}