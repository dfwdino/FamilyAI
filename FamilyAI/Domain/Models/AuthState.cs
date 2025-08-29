namespace FamilyAI.Domain.Models
{
    public static class AuthState
    {
        public static AuthenticatedUser? CurrentUser { get; set; }
        public static void Logout() => CurrentUser = null;
    }
}
