using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Authentication;

public class SetNewPasswordController : ApiControllerBase
{
    [HttpPut]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetNewPassword([FromBody] SetNewPasswordCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public record SetNewPasswordCommand(string PasswordResetCode, string NewPassword) : IRequest<Unit>;

public class SetNewPasswordCommandValidator : AbstractValidator<SetNewPasswordCommand>
{
    public SetNewPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(64);
    }
}

internal class SetNewPasswordCommandHandler : IRequestHandler<SetNewPasswordCommand, Unit>
{
    private readonly ComUnityContext _context;

    public SetNewPasswordCommandHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SetNewPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => 
            x.SecurityCode == request.PasswordResetCode 
            && x.SecurityCodeExpirationDate > DateTime.UtcNow,
            cancellationToken);

        if (user == null)
        {
            throw new BusinessRuleException("Invalid reset code.");
        }

        var hashedPassword = Argon2.Hash(request.NewPassword);
        user.SetNewPassword(hashedPassword);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
