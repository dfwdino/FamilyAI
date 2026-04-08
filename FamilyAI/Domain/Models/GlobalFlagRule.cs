using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("GlobalFlagRules", Schema = "Chat")]
    public class GlobalFlagRule : BasicModel
    {
        [Required]
        public FlagRuleType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Value { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
