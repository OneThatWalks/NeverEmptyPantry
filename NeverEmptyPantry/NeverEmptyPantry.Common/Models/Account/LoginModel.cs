using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}