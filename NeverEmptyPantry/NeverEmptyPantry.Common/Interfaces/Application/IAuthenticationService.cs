using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAuthenticationService
    {
        Task<IOperationResult<TokenModel>> AuthenticateAsync(LoginModel model);

        string GetUserId();
    }
}