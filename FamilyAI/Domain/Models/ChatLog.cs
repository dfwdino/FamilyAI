using ChatSystem.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ChatLogs", Schema = "Chat")]
    public class ChatLog : BasicModel
    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ThreadId { get; set; }

        [Required]
        public bool IsReply { get; set; } = false;

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual UserModel User { get; set; } = null!;

        [ForeignKey(nameof(ThreadId))]
        public virtual Thread Thread { get; set; } = null!;
    }
}
