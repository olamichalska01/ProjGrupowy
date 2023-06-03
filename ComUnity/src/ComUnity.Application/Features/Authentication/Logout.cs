using ComUnity.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComUnity.Application.Features.Authentication;

public class LogoutController : ApiControllerBase
{
    [HttpGet("/api/auth/logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await Mediator.Send(new LogoutCommand(), cancellationToken);
        return Ok();
    }

    public record LogoutCommand() : IRequest<Unit>;

    internal class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutCommandHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Unit.Value;
        }
    }
}
