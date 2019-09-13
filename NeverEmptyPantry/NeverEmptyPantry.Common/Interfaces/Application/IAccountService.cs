using System;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;
using System.Collections.Generic;
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
        /// Gets a user based on a query, if multiple users match the query only the first is returned
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IOperationResult<ProfileModel>> GetProfileAsync(Func<ApplicationUser, bool> query);
        Task<IOperationResult<ProfileModel>> UpdateProfileAsync(ProfileModel model);
    }
}