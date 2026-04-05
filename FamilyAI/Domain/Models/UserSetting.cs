using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("UserSettings", Schema = "Chat")]
    public class UserSetting : BasicModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int PromptTemplateId { get; set; }

        public virtual UserModel User { get; set; } = null!;

        public virtual PromptTemplate PromptTemplate { get; set; } = null!;
    }
}
