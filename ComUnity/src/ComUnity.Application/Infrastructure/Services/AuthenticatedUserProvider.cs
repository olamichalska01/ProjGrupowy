using ComUnity.Application.Database;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ComUnity.Application.Infrastructure.Services;

internal class AuthenticatedUserProvider : IAuthenticatedUserProvider
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ComUnityContext _context;


    public AuthenticatedUserProvider(IHttpContextAccessor contextAccessor, ComUnityContext context)
    {
        _contextAccessor = contextAccessor;
        _context = context;
    }

    public Guid GetUserId()
    {
        var id = _contextAccessor.HttpContext!.User.Claims.First(x => x.Type == "sub").Value;
        return Guid.Parse(id);
    }

    public async Task<UserProfile> GetUserProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var userProfile = await _context.Set<UserProfile>().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return userProfile ?? throw new InvalidOperationException($"Could not find profile for UserId {userId}");
    }
}

public interface IAuthenticatedUserProvider
{
    Guid GetUserId();

    Task<UserProfile> GetUserProfile(CancellationToken cancellationToken);
}
