using NeverEmptyPantry.Common.Models.Account;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAccountService
    {
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="model">The registration model</param>
        /// <returns>A task result that represents the completion of the register operation</returns>
        Task<IOperationResult> RegisterAsync(RegistrationModel model);

        /// <summary>
        /// Gets a user based
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<IOperationResult<ProfileModel>> GetProfileAsync(string username);

        /// <summary>
        /// Updates a user profile
        /// </summary>
        /// <param name="username">The current username of the user</param>
        /// <param name="model">The updated profile</param>
        /// <returns>A task result that represents the completion of the user update</returns>
        Task<IOperationResult<ProfileModel>> UpdateProfileAsync(string username, ProfileModel model);
    }
}