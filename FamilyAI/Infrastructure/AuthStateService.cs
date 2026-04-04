using FamilyAI.Domain.Models;

namespace FamilyAI.Infrastructure
{
    public class AuthStateService
    {
        public AuthenticatedUser? CurrentUser { get; private set; }

        public event Action? OnAuthStateChanged;

        public void SetUser(AuthenticatedUser? user)
        {
            CurrentUser = user;
            OnAuthStateChanged?.Invoke();
        }

        public void Logout() => SetUser(null);
    }
}
