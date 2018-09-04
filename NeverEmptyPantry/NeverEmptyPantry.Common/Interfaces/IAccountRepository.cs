using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> GetUserAsync(string email);

        Task<IEnumerable<OfficeLocation>> GetOfficeLocations();

        Task<IEnumerable<ApplicationUser>> GetAllUsers();
    }
}