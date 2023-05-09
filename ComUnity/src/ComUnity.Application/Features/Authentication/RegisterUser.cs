using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Features.Authentication.Exceptions;
using ComUnity.Application.Features.Authentication.Utils;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Authentication;

public class RegisterUserController : ApiControllerBase
{
    [HttpPost("/api/users")]
    public async Task<ActionResult<Guid>> RegisterUser([FromBody] RegisterUserCommand command)
    {
        return await Mediator.Send(command);
    }
}

public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(320);
    }
}

internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private static int EmailVerificationCodeLength = 64;
    private static int EmailVerificationCodeValidityHours = 24;

    private readonly ComUnityContext _context;

    public RegisterUserCommandHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Set<AuthenticationUser>().AnyAsync(x => x.Email == request.Email, cancellationToken))
        {
            throw new UserAlreadyExistsException();
        }

        var hashedPassword = Argon2.Hash(request.Password);
        var emailVerificationCode = RandomSecurityStringGenerator.Generate(EmailVerificationCodeLength);
        var emailConfirmationCodeExpiration = DateTime.UtcNow.AddHours(EmailVerificationCodeValidityHours);
        var userId = NewId.NextGuid();

        var user = new AuthenticationUser(userId, request.Email, hashedPassword, emailVerificationCode, emailConfirmationCodeExpiration);
        user.DomainEvents.Add(new UserRegisteredEvent(userId));

        _context.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return userId;
    }
}

public class UserRegisteredEvent : DomainEvent
{
    public UserRegisteredEvent(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}