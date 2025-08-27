using ChatSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("PromptValues", Schema = "Chat")]
    public class PromptValue : BasicModel
    {
        [Required]
        public int PromptId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("PrompValue")] // Keeping original column name to match your schema
        public string PromptValueText { get; set; } = string.Empty;


        // Navigation properties
        [ForeignKey(nameof(PromptId))]
        public virtual Prompt Prompt { get; set; } = null!;
    }
}
