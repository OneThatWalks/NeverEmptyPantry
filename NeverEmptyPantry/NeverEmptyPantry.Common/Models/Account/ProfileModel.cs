using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Models.Account
{
    public class ProfileModel
    {
        [EmailAddress]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
        
        public OfficeLocation OfficeLocation { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string Title { get; set; }

        public IDictionary<string, string> Claims { get; set; }

        public string[] Roles { get; set; }

        public ProfileModel() { }

        public ProfileModel(ApplicationUser user)
        {
            Email = user.Email;
            UserName = user.UserName;
            PhoneNumber = user.PhoneNumber;
            OfficeLocation = user.OfficeLocation;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Title = user.Title;
        }

        public ProfileModel AddClaims(IList<Claim> claims)
        {
            Claims = new Dictionary<string, string>();

            foreach (var claim in claims)
            {
                Claims.Add(claim.Type, claim.Value);
            }

            return this;
        }

        public ProfileModel AddRoles(IList<string> roles)
        {
            Roles = roles.ToArray();

            return this;
        }
    }
}