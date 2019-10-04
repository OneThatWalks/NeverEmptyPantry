using NeverEmptyPantry.Common.Models.Account;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IAuthenticationService
    {
        Task<IOperationResult<TokenModel>> AuthenticateAsync(LoginModel model);

        string GetUserId();
    }
}