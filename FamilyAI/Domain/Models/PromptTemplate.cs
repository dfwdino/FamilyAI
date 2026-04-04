using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("PromptTemplates", Schema = "Chat")]
    public class PromptTemplate : BasicModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>Used as fallback when no per-user setting exists.</summary>
        public bool IsDefault { get; set; }
    }
}
