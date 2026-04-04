using FamilyAI.Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FamilyAI.Infrastructure
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthStateService _authStateService;

        public CustomAuthenticationStateProvider(AuthStateService authStateService)
        {
            _authStateService = authStateService;
            _authStateService.OnAuthStateChanged += () =>
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            if (_authStateService.CurrentUser != null)
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, _authStateService.CurrentUser.UserId.ToString()),
                    new Claim(ClaimTypes.Name, _authStateService.CurrentUser.Name),
                    new Claim(ClaimTypes.Role, _authStateService.CurrentUser.RoleName)
                }, "familyauth");
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}
