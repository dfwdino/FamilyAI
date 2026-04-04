using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyAI.Domain.Models
{
    [Table("ParentChildren", Schema = "Chat")]
    public class ParentChild : BasicModel
    {
        [Required]
        public int ParentId { get; set; }

        [Required]
        public int ChildId { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual UserModel Parent { get; set; } = null!;

        [ForeignKey(nameof(ChildId))]
        public virtual UserModel Child { get; set; } = null!;
    }
}
