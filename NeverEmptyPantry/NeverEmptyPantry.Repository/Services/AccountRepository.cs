using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;

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

        public async Task<ApplicationUser> GetUserAsync(string email)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(user => email.Equals(user.Email));

            return appUser;
        }

        public async Task<IEnumerable<OfficeLocation>> GetOfficeLocations()
        {
            var locations = await _context.OfficeLocations.ToListAsync();

            return locations;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }
    }
}