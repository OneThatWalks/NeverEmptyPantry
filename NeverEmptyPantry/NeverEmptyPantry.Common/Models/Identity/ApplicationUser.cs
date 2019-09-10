using Microsoft.AspNetCore.Identity;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Common.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public OfficeLocation OfficeLocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
    }
}