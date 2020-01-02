using Microsoft.AspNetCore.Identity;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Admin;

namespace NeverEmptyPantry.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<OfficeLocation> _officeLocationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IValidator<ProfileModel> _profileValidator;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(IRepository<OfficeLocation> officeLocationRepository, UserManager<ApplicationUser> userManager, IValidator<ProfileModel> profileValidator, RoleManager<IdentityRole> roleManager)
        {
            _officeLocationRepository = officeLocationRepository;
            _userManager = userManager;
            _profileValidator = profileValidator;
            _roleManager = roleManager;
        }

        public async Task<IOperationResult> RegisterAsync(RegistrationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var officeLocation = await _officeLocationRepository.ReadAsync(model.OfficeLocationId);

            if (officeLocation == null)
            {
                var error = new OperationError
                {
                    Name = "Invalid",
                    Description = "Office location not valid"
                };
                return OperationResult.Failed(error);
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OfficeLocation = officeLocation,
                PhoneNumber = model.PhoneNumber,
                Title = model.Title
                // More fields here if needed
            };

            var userManagerResult = await _userManager.CreateAsync(user, model.Password);

            if (!userManagerResult.Succeeded)
            {
                return OperationResult.Failed(userManagerResult.Errors.Select(ie => new OperationError()
                {
                    Name = "UserManager",
                    Description = ie.Description
                }).ToArray());
            }

            return OperationResult.Success;
        }

        public async Task<IOperationResult<ProfileModel>> GetProfileAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError
                {
                    Name = "NotFound",
                    Description = $"User {username} not found"
                });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = _roleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToList();

            var profile = new ProfileModel(user).AddClaims(userClaims);
            var profileRoles = new List<RoleViewModel>();

            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role);

                var roleModel = new RoleViewModel()
                {
                    Name = role.Name,
                    Id = role.Id,
                    Permissions = claims.Select(c => c.Value)
                };

                profileRoles.Add(roleModel);
            }

            profile.Roles = profileRoles;

            return OperationResult<ProfileModel>.Success(profile);
        }

        public async Task<IOperationResult<ProfileModel>> UpdateProfileAsync(string username, ProfileModel model)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var validateResult = _profileValidator.Validate(model);

            if (!validateResult.Succeeded)
            {
                return OperationResult<ProfileModel>.Failed(validateResult.Errors.ToArray());
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return OperationResult<ProfileModel>.Failed(new OperationError
                {
                    Name = "NotFound",
                    Description = $"User {username} not found"
                });
            }

            user.UpdateFromProfile(model);

            var identityUpdateResult = await _userManager.UpdateAsync(user);

            if (!identityUpdateResult.Succeeded)
            {
                return OperationResult<ProfileModel>.Failed(identityUpdateResult.Errors.Select(err =>
                    new OperationError()
                    {
                        Name = "IdentityError",
                        Description = err.Description
                    }).ToArray());
            }

            return OperationResult<ProfileModel>.Success(new ProfileModel(user));
        }
    }
}