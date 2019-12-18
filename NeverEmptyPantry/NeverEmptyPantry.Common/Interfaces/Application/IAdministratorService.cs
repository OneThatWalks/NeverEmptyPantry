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
        /// Updates a role
        /// </summary>
        /// <param name="role">The role model</param>
        /// <returns>A task result that represents the updated role</returns>
        Task<IOperationResult<RoleModel>> UpdateRole(RoleModel role);

        /// <summary>
        /// Adds a role
        /// </summary>
        /// <param name="name">The name of the role</param>
        /// <returns>A task result that represents the completion of the add operation</returns>
        Task<IOperationResult<RoleModel>> AddRoleAsync(RoleModel name);

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
        /// <param name="roleName">the role name</param>
        /// <returns>A task result that represents the completion of the add user to role operation</returns>
        Task<IOperationResult<ProfileModel>> UpdateUser(string userId, ProfileModel roleName);
    }
}