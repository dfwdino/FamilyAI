using ChatSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("Roles", Schema = "Chat")]
    public class Role : BasicModel
    {


        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
