using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NeverEmptyPantry.Authorization.Permissions;
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

        public Task<IOperationResult<IEnumerable<string>>> GetPermissionsAsync()
        {
            return Task.FromResult(OperationResult<IEnumerable<string>>.Success(Permissions.All));
        }

        public async Task<IOperationResult> AddPermissionsToRoleAsync(string roleId, IEnumerable<string> permissions)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var role = roles.FirstOrDefault(r => r.Id.Equals(roleId));

            if (role == null)
            {
                return OperationResult.Failed(new OperationError()
                {
                    Name = "NoRoles",
                    Description = "No role exists that match the role id provided"
                });
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var permissionsToAdd = permissions.Where(p => !claims.Any(c => c.Value.Equals(p))).ToList();

            if (permissionsToAdd.Any())
            {
                var tasks = permissionsToAdd.Select(p => _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, p))).ToList();
                await Task.WhenAll(tasks);

                var failed = tasks.Where(t => !t.Result.Succeeded).ToList();

                if (failed.Any())
                {
                    var errors = failed.Select(f => new OperationError()
                    {
                        Name = "IdentityError",
                        Description = f.Result.Errors.First().Description
                    });
                    return OperationResult.Failed(errors.ToArray());
                }
            }

            return OperationResult.Success;
        }

        public async Task<IOperationResult> RemovePermissionsFromRoleAsync(string roleId, IEnumerable<string> permissions)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var role = roles.FirstOrDefault(r => r.Id.Equals(roleId));

            if (role == null)
            {
                return OperationResult.Failed(new OperationError()
                {
                    Name = "NoRoles",
                    Description = "No role exists that match the role id provided"
                });
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var permissionsToRemove = permissions.Where(p => claims.Any(c => c.Value.Equals(p))).ToList();

            if (permissionsToRemove.Any())
            {
                var tasks = permissionsToRemove.Select(p => _roleManager.RemoveClaimAsync(role, new Claim(CustomClaimTypes.Permission, p))).ToList();
                await Task.WhenAll(tasks);

                var failed = tasks.Where(t => !t.Result.Succeeded).ToList();

                if (failed.Any())
                {
                    var errors = failed.Select(f => new OperationError()
                    {
                        Name = "IdentityError",
                        Description = f.Result.Errors.First().Description
                    });
                    return OperationResult.Failed(errors.ToArray());
                }
            }

            return OperationResult.Success;
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