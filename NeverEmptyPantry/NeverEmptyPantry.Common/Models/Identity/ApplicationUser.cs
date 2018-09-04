using Microsoft.AspNetCore.Identity;

namespace NeverEmptyPantry.Common.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int OfficeLocationId { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
    }
}