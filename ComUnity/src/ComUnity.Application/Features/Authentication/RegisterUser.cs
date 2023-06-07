using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Features.Authentication.Exceptions;
using ComUnity.Application.Features.Authentication.Utils;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Authentication;

public class RegisterUserController : ApiControllerBase
{
    [AllowAnonymous]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [HttpPost("/api/users")]
    public async Task<ActionResult<Guid>> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }
}

public record RegisterUserCommand(string Email, string Password, string Username, DateTime DateOfBirth) : IRequest<Guid>;

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
            .MaximumLength(64);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(64);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .Must(x => DateTime.UtcNow.AddYears(-13) > x).WithMessage("You must be at least 13 years old to use this application.");
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

        if (await _context.Set<UserProfile>().AnyAsync(x => x.Username == request.Username, cancellationToken))
        {
            throw new UsernameTakenException();
        }

        var hashedPassword = Argon2.Hash(request.Password);
        var emailVerificationCode = RandomSecurityStringGenerator.Generate(EmailVerificationCodeLength);
        var emailConfirmationCodeExpiration = DateTime.UtcNow.AddHours(EmailVerificationCodeValidityHours);
        var userId = NewId.NextGuid();

        var user = new AuthenticationUser(userId, request.Email, hashedPassword, emailVerificationCode, emailConfirmationCodeExpiration);
        user.DomainEvents.Add(new UserRegisteredEvent(userId, request.Username, request.DateOfBirth));

        _context.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return userId;
    }
}

public class UserRegisteredEvent : DomainEvent
{
    public UserRegisteredEvent(Guid userId, string username, DateTime dateOfBirth)
    {
        UserId = userId;
        Username = username;
        DateOfBirth = dateOfBirth;
    }

    public Guid UserId { get; }

    public string Username { get; }

    public DateTime DateOfBirth { get; }
}