using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ThreadFlagDetails", Schema = "Chat")]
    public class ThreadFlagDetail : BasicModel
    {
        [Required]
        public int ThreadScanResultId { get; set; }

        /// <summary>The specific message that triggered this flag. Null for thread-level AI topic/tone flags.</summary>
        public int? MessageId { get; set; }

        [Required]
        public FlagRuleType RuleType { get; set; }

        [Required]
        [MaxLength(200)]
        public string RuleValue { get; set; } = string.Empty;

        /// <summary>The text snippet that matched (keywords). Null for AI-detected flags.</summary>
        [MaxLength(500)]
        public string? MatchedExcerpt { get; set; }

        [Required]
        public FlagSource Source { get; set; }

        [ForeignKey(nameof(ThreadScanResultId))]
        public virtual ThreadScanResult ScanResult { get; set; } = null!;

        [ForeignKey(nameof(MessageId))]
        public virtual ChatLog? Message { get; set; }
    }
}
