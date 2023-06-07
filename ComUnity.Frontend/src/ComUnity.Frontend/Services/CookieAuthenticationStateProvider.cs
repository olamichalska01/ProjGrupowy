using Blazored.SessionStorage;
using ComUnity.Frontend.Api;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ComUnity.Frontend.Services;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string UserProfileKey = "userProfile";

    private readonly ISessionStorageService _sessionStorage;

    public CookieAuthenticationStateProvider(ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userProfile = await _sessionStorage.GetItemAsync<GetUserInfoResponse>(UserProfileKey);

        if (userProfile is null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.NameIdentifier, userProfile.UserId),
            new Claim(ClaimTypes.Email, userProfile.UserEmail),
            new Claim(ClaimTypes.Role, userProfile.UserRole)
        }, "CookieAuth");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SetAuthInfo(GetUserInfoResponse userProfile)
    {
        await _sessionStorage.SetItemAsync(UserProfileKey, userProfile);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task ClearIdentity()
    {
        await _sessionStorage.RemoveItemAsync(UserProfileKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
