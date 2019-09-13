using System;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Application.Services
{
    public class AccountService : IAccountService
    {
        public async Task<IOperationResult> RegisterAsync(RegistrationModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<IOperationResult<ProfileModel>> GetProfileAsync(Func<ApplicationUser, bool> query)
        {
            throw new NotImplementedException();
        }

        public async Task<IOperationResult<ProfileModel>> UpdateProfileAsync(ProfileModel model)
        {
            throw new NotImplementedException();
        }
    }
}