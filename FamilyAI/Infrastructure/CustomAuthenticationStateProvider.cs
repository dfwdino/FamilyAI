using FamilyAI.Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FamilyAI.Infrastructure
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            if (AuthState.CurrentUser != null)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, AuthState.CurrentUser.Name),
                    new Claim(ClaimTypes.Role, AuthState.CurrentUser.RoleName)
                }, "apiauth");
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
