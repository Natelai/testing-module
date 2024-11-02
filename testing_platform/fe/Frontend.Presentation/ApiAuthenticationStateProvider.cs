using System.IdentityModel.Tokens.Jwt;
using Frontend.Presentation.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;

namespace Frontend.Presentation;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _authService;

    public ApiAuthenticationStateProvider(AuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authService.GetToken();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var claims = jsonToken!.Claims.ToList();
        return claims;
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        var identity = new ClaimsIdentity();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }
}