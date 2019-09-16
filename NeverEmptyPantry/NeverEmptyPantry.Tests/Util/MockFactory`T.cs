﻿using Microsoft.AspNetCore.Identity;
using Moq;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Entity;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace NeverEmptyPantry.Tests.Util
{
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
                .Setup(_ => _.DeleteAsync(It.IsAny<T>(), It.IsAny<string>()))
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
                OfficeLocation = null,
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

            mgr.Setup(_ => _.GetClaimsAsync(It.IsAny<TUser>()))
                .ReturnsAsync(new List<Claim>())
                .Verifiable();

            mgr.Setup(_ => _.GetRolesAsync(It.IsAny<TUser>()))
                .ReturnsAsync(new List<string>())
                .Verifiable();

            mgr.Setup(_ => _.UpdateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success)
                .Verifiable();

            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
        {
            store = store ?? new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>>();
            roles.Add(new RoleValidator<TRole>());
            return new Mock<RoleManager<TRole>>(store, roles, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null);
        }

        public static Mock<IValidator<T>> GetMockValidator<T>()
        {
            var service = new Mock<IValidator<T>>();

            service.Setup(_ => _.Validate(It.IsAny<T>()))
                .Returns(OperationResult.Success)
                .Verifiable();

            return service;
        }
    }
}
