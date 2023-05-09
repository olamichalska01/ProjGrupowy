using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Features.Authentication.Exceptions;
using ComUnity.Application.Features.Authentication.Settings;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ComUnity.Application.Features.Authentication.Utils;

public class LoginController : ApiControllerBase
{
    [HttpPost("/api/auth/login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
    {
        return await Mediator.Send(command);
    }
}

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record LoginResponse(string Token);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
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

internal class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ComUnityContext _context;

    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(ComUnityContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            throw new InvalidEmailOrPasswordException();
        }

        if (!user.IsEmailVerified)
        {
            throw new EmailNotVerifiedException();
        }

        if (!Argon2.Verify(user.HashedPassword, request.Password))
        {
            throw new InvalidEmailOrPasswordException();
        }

        return new LoginResponse(GenerateJwtToken(user));
    }

    private string GenerateJwtToken(AuthenticationUser user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials
                    (new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                    SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        return stringToken;
    }
}
