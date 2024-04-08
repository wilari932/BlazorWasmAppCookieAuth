using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using wsamcookieauth.Shared;

namespace wsamcookieauth.Client.Provider;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly IHttpClientFactory _httpClientFactory;
    private UserProfileDto? _userProfileDto;
    public CustomAuthStateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_userProfileDto == null)
        {
            var client = _httpClientFactory.CreateClient
                ("ServerAPI");
            var response = await client.GetAsync("/api/Auth/user-profile");
            if (response.IsSuccessStatusCode)
            {
                _userProfileDto =
                    await response.Content.ReadFromJsonAsync<UserProfileDto>();
                var identity = new ClaimsIdentity(new[]{
                    new Claim(ClaimTypes.Email, _userProfileDto?.Email ?? ""),
                    new Claim(ClaimTypes.Role, _userProfileDto?.Role ?? ""),
                    new Claim(ClaimTypes.Name, _userProfileDto ?.Name ?? ""),
                    new Claim("UserId", _userProfileDto?.ToString() ?? "")
                }, "AuthCookie");

                claimsPrincipal = new ClaimsPrincipal(identity);
                //NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthenticationState(claimsPrincipal);
            }
        }
        return new AuthenticationState(claimsPrincipal);
    }

    public void SignOut()
    {
        _userProfileDto = null;
        claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}