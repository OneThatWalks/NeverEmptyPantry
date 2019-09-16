using Microsoft.AspNetCore.Identity;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Common.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public OfficeLocation OfficeLocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public void UpdateFromProfile(ProfileModel profile)
        {
            if (profile.UserName != null)
            {
                UserName = profile.UserName;
            }

            if (profile.OfficeLocation != null)
            {
                OfficeLocation = profile.OfficeLocation;
            }

            if (profile.Email != null)
            {
                Email = profile.Email;
            }

            if (profile.FirstName != null)
            {
                FirstName = profile.FirstName;
            }

            if (profile.LastName != null)
            {
                LastName = profile.LastName;
            }

            if (profile.PhoneNumber != null)
            {
                PhoneNumber = profile.PhoneNumber;
            }

            if (profile.Title != null)
            {
                Title = profile.Title;
            }
        }
    }
}