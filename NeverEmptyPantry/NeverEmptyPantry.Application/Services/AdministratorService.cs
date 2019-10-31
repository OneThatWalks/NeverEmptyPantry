using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Admin;

namespace NeverEmptyPantry.Application.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly ILogger<IAdministratorService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdministratorService(ILogger<IAdministratorService> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IOperationResult<IEnumerable<RoleModel>>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            if (!roles.Any())
            {
                return OperationResult<IEnumerable<RoleModel>>.Failed(new OperationError()
                {
                    Name = "NoRoles",
                    Description = "No roles were found"
                });
            }

            var tasks = roles.Select(async role => new RoleModel()
            {
                RoleId = role.Id,
                Name = role.Name,
                Claims = await _roleManager.GetClaimsAsync(role)
            }).ToList();

            await Task.WhenAll(tasks);

            var results = tasks.Select(t => t.Result);

            return OperationResult<IEnumerable<RoleModel>>.Success(results);
        }

        public async Task<IOperationResult<IEnumerable<string>>> GetPermissionsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult> AddPermissionsToRoleAsync(string roleId, IEnumerable<string> permissions)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<string> permissions)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult<RoleModel>> AddRoleAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult<RoleModel>> RemoveRoleAsync(string name)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult<IEnumerable<ProfileModel>>> GetUsersAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult<ProfileModel>> AddUserToRoleAsync(string userId, string roleId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IOperationResult<ProfileModel>> RemoveUserFromRoleAsync(string userId, string roleId)
        {
            throw new System.NotImplementedException();
        }
    }
}