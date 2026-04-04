using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("Users", Schema = "Chat")]
    public class UserModel : BasicModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public virtual UserPermission? UserPermissions { get; set; }
        public virtual ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();

        // Parent-child relationships
        [InverseProperty(nameof(ParentChild.Parent))]
        public virtual ICollection<ParentChild> ChildLinks { get; set; } = new List<ParentChild>();

        [InverseProperty(nameof(ParentChild.Child))]
        public virtual ICollection<ParentChild> ParentLinks { get; set; } = new List<ParentChild>();
    }
}
