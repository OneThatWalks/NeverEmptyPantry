﻿using Microsoft.AspNetCore.Identity;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using MockFactory = NeverEmptyPantry.Tests.Util.MockFactory;

namespace NeverEmptyPantry.Tests.Application
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AccountServiceTests
    {
        private AccountService _accountService;
        private Mock<IRepository<OfficeLocation>> _mockOfficeRepo;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IValidator<ProfileModel>> _mockProfileValidator;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;


        [SetUp]
        public void SetUp()
        {
            _mockOfficeRepo = MockFactory.GetMockRepository<OfficeLocation>();
            _mockUserManager = MockFactory.MockUserManager<ApplicationUser>();
            _mockProfileValidator = MockFactory.GetMockValidator<ProfileModel>();
            _mockRoleManager = MockFactory.MockRoleManager<IdentityRole>();

            _accountService = new AccountService(_mockOfficeRepo.Object, _mockUserManager.Object, _mockProfileValidator.Object, _mockRoleManager.Object);
        }

        #region RegisterAsync

        [Test]
        public async Task RegisterAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var model = new RegistrationModel
            {
                Username = "TestUser",
                Email = "TestEmail",
                Password = "TestPassword",
                FirstName = "Test",
                LastName = "Test",
                OfficeLocationId = 1,
                PhoneNumber = "18001111111",
                Title = "Test"
            };

            // Act
            var result = await _accountService.RegisterAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public void RegisterAsync_ThrowsException_WhenArgNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _accountService.RegisterAsync(null));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public async Task RegisterAsync_ReturnsFailed_WhenOfficeNotExists()
        {
            // Arrange
            _mockOfficeRepo.Setup(s => s.ReadAsync(It.IsAny<int>())).ReturnsAsync((OfficeLocation)null).Verifiable();
            var model = new RegistrationModel
            {
                Username = "TestUser",
                Email = "TestEmail",
                Password = "TestPassword",
                FirstName = "Test",
                LastName = "Test",
                OfficeLocationId = 1,
                PhoneNumber = "18001111111",
                Title = "Test"
            };

            // Act
            var result = await _accountService.RegisterAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task RegisterAsync_ReturnsFailed_WhenUserManagerFails()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError())).Verifiable();
            var model = new RegistrationModel
            {
                Username = "TestUser",
                Email = "TestEmail",
                Password = "TestPassword",
                FirstName = "Test",
                LastName = "Test",
                OfficeLocationId = 1,
                PhoneNumber = "18001111111",
                Title = "Test"
            };

            // Act
            var result = await _accountService.RegisterAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion RegisterAsync

        #region GetProfileAsync

        // TODO: Re-test

        #endregion GetProfileAsync

        #region UpdateProfileAsync

        [Test]
        public void UpdateProfileAsync_ThrowsException_WhenModelArgumentNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _accountService.UpdateProfileAsync("Test", null));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public void UpdateProfileAsync_ThrowsException_WhenStringArgumentNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _accountService.UpdateProfileAsync(null, new ProfileModel()));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsFailed_WhenValidateFails()
        {
            // Arrange
            _mockProfileValidator.Setup(_ => _.Validate(It.IsAny<ProfileModel>())).Returns(OperationResult.Failed());

            // Act
            var result = await _accountService.UpdateProfileAsync("NotValidUsername", new ProfileModel());

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsFailed_WhenUserNotFound()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            var model = new ProfileModel
            {
                UserName = "TestUser",
                Email = "Test@Email.com"
            };

            // Act
            var result = await _accountService.UpdateProfileAsync("NotValidUsername", model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsFailed_WhenUserManagerFails()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Failed());
            var model = new ProfileModel
            {
                UserName = "TestUser",
                Email = "Test@Email.com"
            };

            // Act
            var result = await _accountService.UpdateProfileAsync("TestUser", model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var model = new ProfileModel
            {
                UserName = "TestUser",
                Email = "Test@Email.com"
            };

            // Act
            var result = await _accountService.UpdateProfileAsync("TestUser", model);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task UpdateProfileAsync_ReturnsExpectedProfile_WhenSuccessful()
        {
            // Arrange
            var model = new ProfileModel
            {
                UserName = "TestUser",
                Email = "Test@Domain.com"
            };

            // Act
            var result = await _accountService.UpdateProfileAsync("TestUser", model);

            // Assert
            Assert.That(result.Data.Email, Is.EqualTo(model.Email));
        }

        #endregion UpdateProfileAsync
    }
}