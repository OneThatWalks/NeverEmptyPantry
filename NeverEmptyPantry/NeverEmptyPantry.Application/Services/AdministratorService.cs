using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Application.Services
{
    public class AdministratorService : IAdministratorService
    {
        // TODO: Add logging
        private readonly ILogger<IAdministratorService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorService(ILogger<IAdministratorService> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
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

        public async Task<IOperationResult> AddPermissionsToRoleAsync(string roleName, IEnumerable<string> permissions)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var role = roles.FirstOrDefault(r => r.Name.Equals(roleName));

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

        public async Task<IOperationResult> RemovePermissionsFromRoleAsync(string roleName, IEnumerable<string> permissions)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var role = roles.FirstOrDefault(r => r.Name.Equals(roleName));

            if (role == null)
            {
                return OperationResult.Failed(new OperationError()
                {
                    Name = "NoRoles",
                    Description = "No role exists that match the role name provided"
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
            var roles = await _roleManager.Roles.ToListAsync();

            if (roles.Exists(r => r.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return OperationResult<RoleModel>.Failed(new OperationError()
                { Name = "RoleExists", Description = "Specified role already exists" });
            }

            var role = new IdentityRole(name);

            var identityResult = await _roleManager.CreateAsync(role);

            if (!identityResult.Succeeded)
            {

                return OperationResult<RoleModel>.Failed(identityResult.Errors.Select(err => new OperationError()
                { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            role = _roleManager.Roles.FirstOrDefault(r => r.Name.Equals(name));
            var claims = await _roleManager.GetClaimsAsync(role);

            var roleModel = new RoleModel()
            {
                Name = role?.Name,
                RoleId = role?.Id,
                Claims = claims
            };

            return OperationResult<RoleModel>.Success(roleModel);
        }

        public async Task<IOperationResult<RoleModel>> RemoveRoleAsync(string name)
        {
            var role = _roleManager.Roles.FirstOrDefault(r =>
                r.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (role == null)
            {
                return OperationResult<RoleModel>.Failed(new OperationError()
                    { Name = "NoRole", Description = "Specified role does not exist" });
            }

            var identityResult = await _roleManager.DeleteAsync(role);

            if (!identityResult.Succeeded)
            {
                return OperationResult<RoleModel>.Failed(identityResult.Errors.Select(err => new OperationError()
                    { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            var roleModel = new RoleModel()
            {
                RoleId = role?.Id,
                Name = role?.Name
            };

            return OperationResult<RoleModel>.Success(roleModel);
        }

        public async Task<IOperationResult<IEnumerable<ProfileModel>>> GetUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            var profiles = users.Select(u => new { Profile = new ProfileModel(u), ApplicationUser = u});

            var enumeratedProfiles = profiles.ToList();
            Parallel.ForEach(enumeratedProfiles, async (profile) =>
            {
                profile.Profile.AddRoles(await _userManager.GetRolesAsync(profile.ApplicationUser));
                profile.Profile.AddClaims(await _userManager.GetClaimsAsync(profile.ApplicationUser));
            });

            return OperationResult<IEnumerable<ProfileModel>>.Success(enumeratedProfiles.Select(p => p.Profile));
        }

        public async Task<IOperationResult<ProfileModel>> AddUserToRoleAsync(string userId, string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError()
                    {Name = "NoRole", Description = "Specified role does not exist"});
            }

            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(userId));

            if (user == null)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError()
                    { Name = "NoUser", Description = "Specified user does not exist" });
            }

            var identityResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!identityResult.Succeeded)
            {
                return OperationResult<ProfileModel>.Failed(identityResult.Errors.Select(err => new OperationError()
                    { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            var profile = new ProfileModel(user);
            profile.AddRoles(await _userManager.GetRolesAsync(user));
            profile.AddClaims(await _userManager.GetClaimsAsync(user));

            return OperationResult<ProfileModel>.Success(profile);
        }

        public async Task<IOperationResult<ProfileModel>> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError()
                    { Name = "NoRole", Description = "Specified role does not exist" });
            }

            var user = _userManager.Users.FirstOrDefault(u => u.Id.Equals(userId));

            if (user == null)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError()
                    { Name = "NoUser", Description = "Specified user does not exist" });
            }

            var identityResult = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (!identityResult.Succeeded)
            {
                return OperationResult<ProfileModel>.Failed(identityResult.Errors.Select(err => new OperationError()
                    { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            var profile = new ProfileModel(user);
            profile.AddRoles(await _userManager.GetRolesAsync(user));
            profile.AddClaims(await _userManager.GetClaimsAsync(user));

            return OperationResult<ProfileModel>.Success(profile);
        }
    }
}