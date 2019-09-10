using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAccountService
    {
        Task<LoginResult> LoginAsync(LoginDto model);
        Task<RegistrationResult> RegisterAsync(RegisterDto model);
        Task<ProfileResult> GetProfileAsync(string email);
        Task<ApplicationUser> GetUserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
        Task<ProfileResult> SetProfileAsync(ProfileDto model);
        Task<LogoutResult> LogoutAsync();
        Task<IEnumerable<OfficeLocation>> GetOfficeLocations();

        Task<IEnumerable<IdentityRole>> GetRoles();
        Task<IEnumerable<string>> GetUserRoles(string email);
        Task<IdentityResult> CreateRole(string name);
        Task<IdentityResult> RemoveRole(string name);
        Task<IdentityResult> UpdateRole(string name, string newName);
        Task<ProfileResult> AddUserToRole(string email, string roleName);
        Task<ProfileResult> RemoveUserFromRole(string email, string roleName);

        Task<IEnumerable<ApplicationUser>> GetAllUsers();
    }
}