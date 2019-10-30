using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Admin;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAdministratorService
    {
        Task<IOperationResult<IEnumerable<RoleModel>>> GetRolesAsync();
        Task<IOperationResult<IEnumerable<string>>> GetPermissionsAsync();
        Task<IOperationResult> AddPermissionsToRoleAsync(string roleId, IEnumerable<string> permissions);
        Task<IOperationResult> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<string> permissions);
        Task<IOperationResult<RoleModel>> AddRoleAsync(string name);
        Task<IOperationResult<RoleModel>> RemoveRoleAsync(string name);
        Task<IOperationResult<IEnumerable<ProfileModel>>> GetUsersAsync();
        Task<IOperationResult<ProfileModel>> AddUserToRoleAsync(string userId, string roleId);
        Task<IOperationResult<ProfileModel>> RemoveUserFromRoleAsync(string userId, string roleId);
    }
}