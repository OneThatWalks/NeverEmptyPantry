using System.Collections.Generic;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.WebUi.Models
{
    public class ProfileViewModel
    {
        public string Email { get; set; } // Allow only this

        public string PhoneNumber { get; set; }

        public OfficeLocation OfficeLocation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public IEnumerable<string> Roles { get; set; }
        public string RolesAsCsv => Roles != null ? string.Join(',', Roles) : "";

        public IEnumerable<string> AllRoles { get; set; }
    }
}