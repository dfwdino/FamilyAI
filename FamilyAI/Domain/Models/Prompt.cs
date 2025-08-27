using ChatSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("Prompts", Schema = "Chat")]
    public class Prompt : BasicModel
    {

        [Required]
        [MaxLength(100)]
        public string PromptTitle { get; set; } = string.Empty;


        // Navigation properties
        public virtual ICollection<PromptValue> PromptValues { get; set; } = new List<PromptValue>();
    }
}
