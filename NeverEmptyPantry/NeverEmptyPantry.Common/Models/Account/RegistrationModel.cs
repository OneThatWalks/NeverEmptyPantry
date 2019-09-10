using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class RegistrationModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public int OfficeLocationId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
         
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}