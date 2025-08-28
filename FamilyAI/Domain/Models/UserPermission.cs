using ChatSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("UserPermissions", Schema = "Chat")]
    public class UserPermission : BasicModel
    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }


        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual UserModel User { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;
    }
}
