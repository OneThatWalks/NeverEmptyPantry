﻿using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class RegistrationModel : LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public int OfficeLocationId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Title { get; set; }
    }
}