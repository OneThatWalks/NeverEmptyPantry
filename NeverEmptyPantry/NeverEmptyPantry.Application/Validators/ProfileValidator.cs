using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Application.Validators
{
    public class ProfileValidator : IValidator<ProfileModel>
    {
        public IOperationResult Validate(ProfileModel obj)
        {
            return OperationResult.Success;
        }
    }
}