using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Admin;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAdministratorService
    {
        /// <summary>
        /// Gets the list of roles
        /// </summary>
        /// <returns>A task result that represents the available roles</returns>
        Task<IOperationResult<IEnumerable<RoleModel>>> GetRolesAsync();

        /// <summary>
        /// Gets available permissions
        /// </summary>
        /// <returns>A task result that represents the available permissions</returns>
        Task<IOperationResult<IEnumerable<string>>> GetPermissionsAsync();

        /// <summary>
        /// Adds permissions to a role
        /// </summary>
        /// <param name="roleId">The role identifier</param>
        /// <param name="permissions">The list of permissions</param>
        /// <returns>A task result that represents the result of the add operation</returns>
        Task<IOperationResult> AddPermissionsToRoleAsync(string roleId, IEnumerable<string> permissions);

        /// <summary>
        /// Removes permissions from a role
        /// </summary>
        /// <param name="roleId">The role identifier</param>
        /// <param name="permissions">The list of permissions</param>
        /// <returns>A task result that represents the result of the remove operation</returns>
        Task<IOperationResult> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<string> permissions);

        /// <summary>
        /// Adds a role
        /// </summary>
        /// <param name="name">The name of the role</param>
        /// <returns>A task result that represents the completion of the add operation</returns>
        Task<IOperationResult<RoleModel>> AddRoleAsync(string name);

        /// <summary>
        /// Removes a role
        /// </summary>
        /// <param name="name">The name of the role</param>
        /// <returns>A task result that represents the completion of the remove operation</returns>
        Task<IOperationResult<RoleModel>> RemoveRoleAsync(string name);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>A task result that represents all of the users</returns>
        Task<IOperationResult<IEnumerable<ProfileModel>>> GetUsersAsync();

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="roleId">the role identifier</param>
        /// <returns>A task result that represents the completion of the add user to role operation</returns>
        Task<IOperationResult<ProfileModel>> AddUserToRoleAsync(string userId, string roleId);

        /// <summary>
        /// Removes a user from a role
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <param name="roleId">the role identifier</param>
        /// <returns>A task result that represents the completion of the remove user from role operation</returns>
        Task<IOperationResult<ProfileModel>> RemoveUserFromRoleAsync(string userId, string roleId);
    }
}