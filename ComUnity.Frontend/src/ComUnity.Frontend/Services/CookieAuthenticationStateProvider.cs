using ComUnity.Frontend.Api;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ComUnity.Frontend.Services;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.CompletedTask;
        return new AuthenticationState(claimsPrincipal);
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetAuthInfo(GetUserInfoResponse userProfile)
    {
        var identity = new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            new Claim(ClaimTypes.Email, userProfile.UserEmail),
            new Claim(ClaimTypes.Role, userProfile.UserRole)
        }, "CookieAuth");

        claimsPrincipal = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void ClearIdentity()
    {
        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
