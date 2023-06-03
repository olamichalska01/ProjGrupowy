using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using ComUnity.Application.Features.Authentication.Exceptions;
using ComUnity.Application.Features.Authentication.Settings;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [AllowAnonymous]
    [HttpPost("/api/auth/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }
}

public record LoginCommand(string Email, string Password, string? AuthenticationType) : IRequest<LoginResponse>;

public record LoginResponse(string? Token);

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public static List<string> AllAuthenticationTypes = new List<string>
    {
        AuthenticationTypes.Cookie,
        AuthenticationTypes.Jwt
    };

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

        RuleFor(x => x.AuthenticationType)
            .Must(x => x is null || AllAuthenticationTypes.Contains(x));
    }
}

internal class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ComUnityContext _context;
    private readonly HttpContext _httpContext;

    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(ComUnityContext context, IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContext = httpContextAccessor.HttpContext;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

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

        var subject = new ClaimsIdentity(new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("role", user.Role)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

        if(request.AuthenticationType == AuthenticationTypes.Cookie)
        {
            await _httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(subject),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            });

            return new LoginResponse(null);
        }

        return new LoginResponse(GenerateJwtToken(subject));
    }

    private string GenerateJwtToken(ClaimsIdentity subject)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
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
