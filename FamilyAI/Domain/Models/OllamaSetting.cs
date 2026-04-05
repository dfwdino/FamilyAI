using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("OllamaSettings", Schema = "Chat")]
    public class OllamaSetting : BasicModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Full URL to the Ollama API, e.g. http://localhost:11434/</summary>
        [Required]
        [MaxLength(500)]
        public string ModelUrl { get; set; } = "http://localhost:11434/";

        /// <summary>Ollama model identifier, e.g. llama3.2-vision</summary>
        [Required]
        [MaxLength(100)]
        public string ModelName { get; set; } = "llama3.2-vision";

        /// <summary>The currently active configuration used for all AI calls.</summary>
        public bool IsActive { get; set; }
    }
}
