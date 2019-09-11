using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Repository.Services
{
    [ExcludeFromCodeCoverage]
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ApplicationUser> GetUserByUserNameAsync(string username)
        {
            return _context.Users.FirstOrDefaultAsync(user => username.Equals(user.UserName));
        }

        public Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(user => email.Equals(user.Email));
        }

        public async Task<IEnumerable<OfficeLocation>> GetOfficeLocations()
        {
            // Await this "pass-through" since the return type is IEnumerable
            var locations = await _context.OfficeLocations.ToListAsync();

            return locations;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            // Await this "pass-through" since the return type is IEnumerable
            var users = await _context.Users.ToListAsync();

            return users;
        }
    }
}