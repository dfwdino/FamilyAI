using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ThreadScanResults", Schema = "Chat")]
    public class ThreadScanResult : BasicModel
    {
        [Required]
        public int ThreadId { get; set; }

        [Required]
        public int ScannedByParentId { get; set; }

        [Required]
        public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

        public bool IsFlagged { get; set; }

        public string? AiSummary { get; set; }

        /// <summary>Hash of the active rule set at scan time. Used to detect stale results.</summary>
        [MaxLength(64)]
        public string RuleSetHash { get; set; } = string.Empty;

        [ForeignKey(nameof(ThreadId))]
        public virtual ThreadModel Thread { get; set; } = null!;

        [ForeignKey(nameof(ScannedByParentId))]
        public virtual UserModel ScannedByParent { get; set; } = null!;

        public virtual ICollection<ThreadFlagDetail> FlagDetails { get; set; } = new List<ThreadFlagDetail>();
    }
}
