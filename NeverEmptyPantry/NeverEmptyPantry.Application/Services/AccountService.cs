using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Util;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;

        public AccountService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IAccountRepository accountRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _accountRepository = accountRepository;
        }

        public async Task<LoginResult> LoginAsync(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (!result.Succeeded)
            {
                var errors = new List<OperationError>()
                {
                    new OperationError {Code = "500", Description = "SignInManager failed to sign in user"}
                };
                return LoginResult.LoginFailed(errors.ToArray());
            }

            var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
            var jwt = new JwtDetail
            {
                JwtKey = _configuration["JwtKey"],
                JwtIssuer = _configuration["JwtIssuer"],
                JwtExpireDays = _configuration["JwtExpireDays"]
            };

            var roles = await _userManager.GetRolesAsync(appUser);

            return LoginResult.LoginSuccess(Jwt.GenerateJwtToken(model.Email, appUser, roles, jwt));
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OfficeLocationId = model.OfficeLocationId,
                Title = model.Title,
                // More fields
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = new List<OperationError>() { new OperationError() { Code = "500", Description = "UserManager failed to create user" } };

                errors.AddRange(result.Errors.Select(identityError => new OperationError { Code = identityError.Code, Description = identityError.Description }));

                return RegistrationResult.RegistrationFailed(errors.ToArray());
            }

            await _signInManager.SignInAsync(user, false);

            var jwt = new JwtDetail
            {
                JwtKey = _configuration["JwtKey"],
                JwtIssuer = _configuration["JwtIssuer"],
                JwtExpireDays = _configuration["JwtExpireDays"]
            };

            var roles = await _userManager.GetRolesAsync(user);

            return RegistrationResult.RegistrationSuccess(Jwt.GenerateJwtToken(model.Email, user, roles, jwt));
        }

        public async Task<ProfileResult> GetProfileAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                var err = new OperationError
                {
                    Code = "501",
                    Description = "Query was null or empty"
                };
                return ProfileResult.ProfileFailed(err);
            }

            var account = await _accountRepository.GetUserAsync(email);

            if (account == null)
            {
                var err = new OperationError
                {
                    Code = "502",
                    Description = "No account found with supplied query"
                };
                return ProfileResult.ProfileFailed(err);
            }

            var profile = ProfileDto.From(account);

            return ProfileResult.ProfileSuccess(profile);
        }

        public async Task<ApplicationUser> GetUserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);

            return user;
        }

        public async Task<ProfileResult> SetProfileAsync(ProfileDto model)
        {
            var user = await _accountRepository.GetUserAsync(model.Email);

            if (user == null)
            {
                var err = new OperationError
                {
                    Code = "500",
                    Description = "Could not retrieve user"
                };
                return ProfileResult.ProfileFailed(err);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.OfficeLocationId = model.OfficeLocationId;
            user.Title = model.Title;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = new List<OperationError>();

                errors.AddRange(result.Errors.Select(identityError => new OperationError { Code = identityError.Code, Description = identityError.Description }));

                var err = new OperationError
                {
                    Code = "500",
                    Description = "Could not update user"
                };

                errors.Add(err);
                return ProfileResult.ProfileFailed(errors.ToArray());
            }

            var profile = ProfileDto.From(user);

            return ProfileResult.ProfileSuccess(profile);
        }

        public async Task<LogoutResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            return LogoutResult.LogoutSuccess;
        }

        public async Task<IEnumerable<OfficeLocation>> GetOfficeLocations()
        {
            return await _accountRepository.GetOfficeLocations();
        }

        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            var identityRoles = _roleManager.Roles;

            return identityRoles.ToList();
        }

        public async Task<IEnumerable<string>> GetUserRoles(string email)
        {
            var user = await _accountRepository.GetUserAsync(email);

            var userRoleNames = await _userManager.GetRolesAsync(user);

            return userRoleNames;
        }

        public async Task<IdentityResult> CreateRole(string name)
        {
            var role = new IdentityRole(name);

            var roleCreationResult = await _roleManager.CreateAsync(role);

            return roleCreationResult;
        }

        public async Task<IdentityResult> RemoveRole(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Description = $"Role {name} not found",
                    Code = "400"
                });
            }

            var roleDeletionResult = await _roleManager.DeleteAsync(role);

            return roleDeletionResult;
        }

        public async Task<IdentityResult> UpdateRole(string name, string newName)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Description = $"Role {name} not found",
                    Code = "400"
                });
            }

            role.Name = newName;

            var roleUpdateResult = await _roleManager.UpdateAsync(role);

            return roleUpdateResult;
        }

        public async Task<ProfileResult> AddUserToRole(string email, string roleName)
        {
            var user = await _accountRepository.GetUserAsync(email);

            if (user == null)
            {
                // I can't see how this might happen
                return ProfileResult.ProfileFailed(new OperationError
                {
                    Description = "User manager failed to find user",
                    Code = "100"
                });
            }

            var roleUserResult = await _userManager.AddToRoleAsync(user, roleName);

            if (roleUserResult.Succeeded)
                return ProfileResult.ProfileSuccess(
                    ProfileDto.From(await _accountRepository.GetUserAsync(email)));

            var newErrorArray = roleUserResult.Errors.Select(e => new OperationError
            {
                Description = e.Description,
                Code = e.Code
            }).ToList();
            newErrorArray.Add(new OperationError
            {
                Description = "User manager failed to add role to user",
                Code = "100"
            });
            return ProfileResult.ProfileFailed(newErrorArray.ToArray());

        }

        public async Task<ProfileResult> RemoveUserFromRole(string email, string roleName)
        {
            var user = await _accountRepository.GetUserAsync(email);

            if (user == null)
            {
                // I can't see how this might happen
                return ProfileResult.ProfileFailed(new OperationError
                {
                    Description = "User manager failed to find user",
                    Code = "100"
                });
            }

            var roleUserResult = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (roleUserResult.Succeeded)
                return ProfileResult.ProfileSuccess(
                    ProfileDto.From(await _accountRepository.GetUserAsync(email)));

            var newErrorArray = roleUserResult.Errors.Select(e => new OperationError
            {
                Description = e.Description,
                Code = e.Code
            }).ToList();
            newErrorArray.Add(new OperationError
            {
                Description = "User manager failed to remove role from user",
                Code = "100"
            });
            return ProfileResult.ProfileFailed(newErrorArray.ToArray());

        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var users = await _accountRepository.GetAllUsers();

            return users;
        }
    }
}