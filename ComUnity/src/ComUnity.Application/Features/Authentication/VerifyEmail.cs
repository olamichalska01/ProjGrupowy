using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Authentication;

public class VerifyEmailController : ApiControllerBase
{
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("/api/users/verifyEmail")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public record VerifyEmailCommand(string VerificationCode) : IRequest;

internal class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly ComUnityContext _context;

    public VerifyEmailCommandHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Set<AuthenticationUser>().FirstOrDefaultAsync(x => x.SecurityCode == request.VerificationCode, cancellationToken);

        if (user == null || user.SecurityCodeExpirationDate < DateTime.UtcNow)
        {
            throw new InvalidVerificationCodeException();
        }

        user.VerifyEmail();
        await _context.SaveChangesAsync(cancellationToken);
    }
}