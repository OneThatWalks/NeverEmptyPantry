using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Entity;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Identity;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Tests.Util
{
    [ExcludeFromCodeCoverage]
    public static class MockFactory
    {
        public static Mock<IRepository<T>> GetMockRepository<T>() where T : IBaseEntity, new()
        {
            var service = new Mock<IRepository<T>>();

            service
                .Setup(_ => _.ReadAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var thing = new T { Id = id };

                    // ReSharper disable once PossibleNullReferenceException

                    return thing;
                })
                .Verifiable();

            service
                .Setup(_ => _.CreateAsync(It.IsAny<T>(), It.IsAny<string>()))
                .ReturnsAsync((T entity, string user) => entity)
                .Verifiable();

            service
                .Setup(_ => _.RemoveAsync(It.IsAny<T>(), It.IsAny<string>()))
                .ReturnsAsync((T entity, string user) => entity)
                .Verifiable();

            service
                .Setup(_ => _.UpdateAsync(It.IsAny<T>(), It.IsAny<string>()))
                .ReturnsAsync((T entity, string user) => entity)
                .Verifiable();

            return service;
        }

        public static ApplicationUser GetApplicationUser(string str)
        {
            return new ApplicationUser()
            {
                Id = "1",
                UserName = str,
                Email = "Test@Email.com",
                FirstName = "Test",
                LastName = "User",
                OfficeLocation = new OfficeLocation()
                {
                    Name = "Office 1",
                    Id = 1
                },
                Title = "Test",
                PhoneNumber = "8008008888"
            };
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(_ => _.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string str) => GetApplicationUser(str) as TUser)
                .Verifiable();

            mgr.Setup(_ => _.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string str) => GetApplicationUser(str) as TUser)
                .Verifiable();

            mgr.Setup(_ => _.GetClaimsAsync(It.IsAny<TUser>()))
                .ReturnsAsync(new List<Claim>())
                .Verifiable();

            mgr.Setup(_ => _.GetRolesAsync(It.IsAny<TUser>()))
                .ReturnsAsync(new List<string>())
                .Verifiable();

            mgr.Setup(_ => _.UpdateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.RemoveFromRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.AddClaimsAsync(It.IsAny<TUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.RemoveClaimsAsync(It.IsAny<TUser>(), It.IsAny<IEnumerable<Claim>>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.AddToRolesAsync(It.IsAny<TUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            mgr.Setup(_ => _.RemoveFromRolesAsync(It.IsAny<TUser>(), It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class, new()
        {
            store ??= new Mock<IRoleStore<TRole>>().Object;
            var roleValidators = new List<IRoleValidator<TRole>> { new RoleValidator<TRole>() };
            var service =  new Mock<RoleManager<TRole>>(store, roleValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null);

            service.Setup(_ => _.GetClaimsAsync(It.IsAny<TRole>())).ReturnsAsync(new List<Claim>() { new Claim(CustomClaimTypes.Permission, Permissions.Users.Create) }).Verifiable();
            service.Setup(_ => _.AddClaimAsync(It.IsAny<TRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            service.Setup(_ => _.RemoveClaimAsync(It.IsAny<TRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            service.Setup(_ => _.CreateAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            service.Setup(_ => _.DeleteAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            service.Setup(_ => _.UpdateAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            service.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true).Verifiable();
            service.Setup(_ => _.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new TRole()).Verifiable();
            service.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new TRole()).Verifiable();

            return service;
        }

        public static Mock<IValidator<T>> GetMockValidator<T>()
        {
            var service = new Mock<IValidator<T>>();

            service.Setup(_ => _.Validate(It.IsAny<T>()))
                .Returns(OperationResult.Success)
                .Verifiable();

            return service;
        }

        public static Mock<ILogger<T>> GetMockLogger<T>()
        {
            var service = new Mock<ILogger<T>>();

            return service;
        }

        public static Mock<IAuthenticationService> GetMockAuthenticationService()
        {
            var service = new Mock<IAuthenticationService>();

            service.Setup(_ => _.GetUserId()).Returns("testuser");

            return service;
        }

        public static Mock<IValidatorFactory<T>> GetMockValidatorFactory<T>(Mock<IValidator<T>> mockValidator)
        {
            var service = new Mock<IValidatorFactory<T>>();

            service.Setup(_ => _.GetValidator<IValidator<T>>()).Returns(mockValidator.Object);

            return service;
        }
    }
}
