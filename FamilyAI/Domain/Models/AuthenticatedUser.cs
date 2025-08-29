namespace FamilyAI.Domain.Models
{
    public class AuthenticatedUser
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();

    }
}
