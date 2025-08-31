using FamilyAI.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("Threads", Schema = "Chat")]
    public class ThreadModel : BasicModel
    {


        [Required]
        [MaxLength(100)]
        public string ThreadName { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }


        // Navigation properties
        public virtual ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual UserModel User { get; set; } = null!;
    }
}
