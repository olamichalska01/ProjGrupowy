using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.UserProfileManagement;

public class IsUsernameTakenController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet("/api/users/exists")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> IsUsernameTaken([FromQuery] string username, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new IsUsernameTakenQuery(username), cancellationToken);
    }
}

public record IsUsernameTakenQuery(string Username) : IRequest<bool>;

internal class IsUsernameTakenQueryHandler : IRequestHandler<IsUsernameTakenQuery, bool>
{
    private readonly ComUnityContext _context;

    public IsUsernameTakenQueryHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(IsUsernameTakenQuery request, CancellationToken cancellationToken)
        => await _context.Set<UserProfile>().AnyAsync(x => x.Username == request.Username, cancellationToken);
}

