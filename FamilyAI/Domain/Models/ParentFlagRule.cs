using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ParentFlagRules", Schema = "Chat")]
    public class ParentFlagRule : BasicModel
    {
        [Required]
        public int ParentUserId { get; set; }

        [Required]
        public FlagRuleType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>Links back to the GlobalFlagRule this was copied from. Null if custom.</summary>
        public int? GlobalRuleId { get; set; }

        /// <summary>Parent can disable a rule without deleting it.</summary>
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(ParentUserId))]
        public virtual UserModel Parent { get; set; } = null!;

        [ForeignKey(nameof(GlobalRuleId))]
        public virtual GlobalFlagRule? GlobalRule { get; set; }
    }
}
