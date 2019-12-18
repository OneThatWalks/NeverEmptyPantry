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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeverEmptyPantry.Application.Services
{
    public class AdministratorService : IAdministratorService
    {
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
            _logger.LogDebug($"Executing {nameof(GetRolesAsync)}");

            var roles = await _roleManager.Roles.ToListAsync();

            if (!roles.Any())
            {
                _logger.LogDebug($"Executed {nameof(GetRolesAsync)}.  Returned NoRoles.");

                return OperationResult<IEnumerable<RoleModel>>.Failed(new OperationError()
                {
                    Name = "NoRoles",
                    Description = "No roles were found"
                });
            }

            var tasks = roles.Select(async role =>
            {
                var permissions = await _roleManager.GetClaimsAsync(role);

                return new RoleModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = permissions.Select(p => p.Value)
                };
            }).ToList();

            await Task.WhenAll(tasks);

            var results = tasks.Select(t => t.Result);

            _logger.LogDebug($"Executed {nameof(GetRolesAsync)}.  Returned Roles.");

            return OperationResult<IEnumerable<RoleModel>>.Success(results);
        }

        public Task<IOperationResult<IEnumerable<string>>> GetPermissionsAsync()
        {
            _logger.LogDebug($"Executed {nameof(GetPermissionsAsync)}.  Returned Permissions.");

            return Task.FromResult(OperationResult<IEnumerable<string>>.Success(Permissions.All));
        }

        public async Task<IOperationResult<RoleModel>> UpdateRole(RoleModel role)
        {
            _logger.LogDebug($"Executing {nameof(UpdateRole)}. Param: {JsonConvert.SerializeObject(role)}");

            var roleManagerRole = _roleManager.Roles.FirstOrDefault(r => r.Id == role.Id);

            if (roleManagerRole == null)
            {
                _logger.LogDebug($"Executed {nameof(UpdateRole)}.  Returned NoRole.");

                return OperationResult<RoleModel>.Failed(new OperationError()
                    { Name = "NoRole", Description = "Specified role does not exist" });
            }

            roleManagerRole.Name = role.Name;
            var updateRoleResult = await _roleManager.UpdateAsync(roleManagerRole);

            if (!updateRoleResult.Succeeded)
            {
                _logger.LogDebug($"Executed {nameof(UpdateRole)}.  Returned IdentityError.");

                return OperationResult<RoleModel>.Failed(updateRoleResult.Errors.Select(err => new OperationError()
                    { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            var claims = await _roleManager.GetClaimsAsync(roleManagerRole);

            var claimsToRemove = claims.Where(c => !role.Permissions.Any(rp => rp.Equals(c.Value))).ToList();
            var claimsToAdd = role.Permissions.Where(rp => !claims.Any(c => c.Value.Equals(rp))).Select(p => new Claim(CustomClaimTypes.Permission, p));

            var removeTasks = claimsToRemove.Select(claim => _roleManager.RemoveClaimAsync(roleManagerRole, claim));
            var addTasks = claimsToAdd.Select(claim => _roleManager.AddClaimAsync(roleManagerRole, claim));

            await Task.WhenAll(removeTasks.Union(addTasks).ToList());

            _logger.LogDebug($"Executed {nameof(UpdateRole)}.  Returned RoleModel.");

            return OperationResult<RoleModel>.Success(role);
        }


        public async Task<IOperationResult<RoleModel>> AddRoleAsync(RoleModel role)
        {
            _logger.LogDebug($"Executing {nameof(AddRoleAsync)}. Param: {JsonConvert.SerializeObject(role)}");

            var roleExists = await _roleManager.Roles.AnyAsync(r =>
                r.Name.Equals(role.Name, StringComparison.InvariantCultureIgnoreCase));

            if (roleExists)
            {
                _logger.LogDebug($"Executed {nameof(AddRoleAsync)}.  Returned RoleExists.");

                return OperationResult<RoleModel>.Failed(new OperationError()
                { Name = "RoleExists", Description = "Specified role already exists" });
            }

            var roleManagerRole = new IdentityRole(role.Name);

            var identityResult = await _roleManager.CreateAsync(roleManagerRole);

            if (!identityResult.Succeeded)
            {
                return OperationResult<RoleModel>.Failed(identityResult.Errors.Select(err => new OperationError()
                { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            roleManagerRole = await _roleManager.FindByNameAsync(role.Name);
            var claims = role.Permissions.Select(p => new Claim(CustomClaimTypes.Permission, p));
            var addTasks = claims.Select(c => _roleManager.AddClaimAsync(roleManagerRole, c)).ToList();
            await Task.WhenAll(addTasks);

            if (addTasks.Any(add => !add.Result.Succeeded))
            {
                _logger.LogDebug($"Executed {nameof(AddRoleAsync)}.  Returned IdentityError.");

                return OperationResult<RoleModel>.Failed(addTasks.SelectMany(t => t.Result.Errors).Select(err => new OperationError()
                    { Name = "IdentityError", Description = err.Description }).ToArray());
            }

            claims = await _roleManager.GetClaimsAsync(roleManagerRole);

            var roleModel = new RoleModel()
            {
                Name = roleManagerRole.Name,
                Id = roleManagerRole.Id,
                Permissions = claims.Select(c => c.Value)
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
                Id = role?.Id,
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
                var userRoles = await _userManager.GetRolesAsync(profile.ApplicationUser);
                var userClaims = await _userManager.GetClaimsAsync(profile.ApplicationUser);
                var roles = _roleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToList();
                profile.Profile.AddClaims(userClaims);

                var profileRoles = new List<RoleModel>();

                foreach (var role in roles)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);

                    var roleModel = new RoleModel()
                    {
                        Name = role.Name,
                        Id = role.Id,
                        Permissions = claims.Select(c => c.Value)
                    };

                    profileRoles.Add(roleModel);
                }

                profile.Profile.Roles = profileRoles;
            });

            return OperationResult<IEnumerable<ProfileModel>>.Success(enumeratedProfiles.Select(p => p.Profile));
        }

        public async Task<IOperationResult<ProfileModel>> UpdateUser(string userId, ProfileModel roleName)
        {
            throw new NotImplementedException();
        }
    }
}