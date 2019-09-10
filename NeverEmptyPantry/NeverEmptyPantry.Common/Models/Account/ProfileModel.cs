using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class ProfileModel
    {
        public string Email { get; set; }

        public string UserName { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public OfficeLocation OfficeLocation { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Title { get; set; }

        public static ProfileModel FromUser(ApplicationUser user)
        {
            return new ProfileModel
            {
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                OfficeLocation = user.OfficeLocation,
                PhoneNumber = user.PhoneNumber,
                Title = user.Title
            };
        }
    }
}