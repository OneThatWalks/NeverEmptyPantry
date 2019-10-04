using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A task result that represents the user</returns>
        Task<ApplicationUser> GetUserByUserNameAsync(string username);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A task result that represents the user</returns>
        Task<ApplicationUser> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets all office locations
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OfficeLocation>> GetOfficeLocations();

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>A task result that represents all of the users</returns>
        Task<IEnumerable<ApplicationUser>> GetAllUsers();

        /// <summary>
        /// Gets the user if user exists otherwise the system user
        /// </summary>
        /// <param name="userId">The user id to query for</param>
        /// <returns>A task result that represents the user</returns>
        Task<ApplicationUser> GetUserOrSystemAsync(string userId);
    }
}