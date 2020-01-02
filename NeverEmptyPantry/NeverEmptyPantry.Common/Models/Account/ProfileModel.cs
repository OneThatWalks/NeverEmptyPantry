using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NeverEmptyPantry.Common.Models.Admin;

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

        public IEnumerable<string> Permissions { get; set; }

        public IEnumerable<RoleViewModel> Roles { get; set; }

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
            Permissions = claims.Select(c => c.Value).ToArray();

            return this;
        }
    }
}