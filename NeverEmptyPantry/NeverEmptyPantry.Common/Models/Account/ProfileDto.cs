using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class ProfileDto
    {
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public int OfficeLocationId { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Title { get; set; }
        
        public string Password { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public static ProfileDto From(ApplicationUser user)
        {
            return new ProfileDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                OfficeLocationId = user.OfficeLocationId,
                PhoneNumber = user.PhoneNumber,
                Title = user.Title
            };
        }
    }
}