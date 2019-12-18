using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Common.Models.Admin
{
    public class RoleModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}