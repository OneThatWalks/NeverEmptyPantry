using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}