using System.Collections.Generic;
using System.Security.Claims;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Common.Models.Admin
{
    public class RoleModel
    {
        public string RoleId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }
}