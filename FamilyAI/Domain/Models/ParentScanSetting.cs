using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ParentScanSettings", Schema = "Chat")]
    public class ParentScanSetting : BasicModel
    {
        [Required]
        public int ParentUserId { get; set; }

        [Required]
        public ScanSensitivity Sensitivity { get; set; } = ScanSensitivity.Medium;

        [ForeignKey(nameof(ParentUserId))]
        public virtual UserModel Parent { get; set; } = null!;
    }
}
