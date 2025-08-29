using FamilyAI.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("Threads", Schema = "Chat")]
    public class Thread : BasicModel
    {


        [Required]
        [MaxLength(100)]
        public string ThreadName { get; set; } = string.Empty;


        // Navigation properties
        public virtual ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();
    }
}
