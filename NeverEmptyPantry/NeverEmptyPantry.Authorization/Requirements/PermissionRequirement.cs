using Microsoft.AspNetCore.Authorization;

namespace NeverEmptyPantry.Authorization.Requirements
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; set; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}