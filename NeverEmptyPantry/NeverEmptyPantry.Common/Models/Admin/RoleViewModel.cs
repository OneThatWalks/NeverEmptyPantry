using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Admin
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}