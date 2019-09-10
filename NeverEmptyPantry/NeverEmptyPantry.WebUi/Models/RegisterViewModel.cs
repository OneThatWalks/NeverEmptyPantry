using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.WebUi.Models
{
    public class RegisterViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int OfficeLocationId { get; set; }

        public List<OfficeLocation> OfficeLocations { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}