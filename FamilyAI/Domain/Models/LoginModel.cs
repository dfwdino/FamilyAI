using System.ComponentModel.DataAnnotations;

namespace FamilyAI.Domain.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

    }
}
