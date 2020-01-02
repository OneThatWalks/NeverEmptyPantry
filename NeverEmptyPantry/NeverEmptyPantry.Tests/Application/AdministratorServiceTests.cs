using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Admin;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Tests.Util;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Account;
using MockFactory = NeverEmptyPantry.Tests.Util.MockFactory;

namespace NeverEmptyPantry.Tests.Application
{
    [TestFixture]
    public class AdministratorServiceTests
    {
        private AdministratorService _administratorService;
        private Mock<ILogger<IAdministratorService>> _mockLogger;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<IAdministratorService>>();
            _mockRoleManager = MockFactory.MockRoleManager<IdentityRole>();

            var roles = new List<IdentityRole>()
            {
                new IdentityRole(DefaultRoles.Administrator) {Id = DefaultRoles.Administrator.ToUpper()},
                new IdentityRole(DefaultRoles.User) {Id = DefaultRoles.User.ToUpper()},
                new IdentityRole("UpdateRole") {Id = "UpdateRole".ToUpper()}
            };
            var asyncRolesEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncRolesEnumerable.AsQueryable()).Verifiable();
            _mockRoleManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityRole(DefaultRoles.Administrator)).Verifiable();

            _mockUserManager = MockFactory.MockUserManager<ApplicationUser>();
            var users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "TestUser",
                    Email = "TestEmail@domain.com",
                    FirstName = "Test",
                    LastName = "User",
                    OfficeLocation = new OfficeLocation()
                    {
                        Name = "Indy",
                        Address = "Address",
                        City = "Indianapolis",
                        Country = "USA",
                        State = "IN",
                        Zip = "46240"
                    },
                    UserName = "TestUser",
                    Title = "Tester"
                }
            };
            var asyncUserEnumerable = new TestAsyncEnumerable<ApplicationUser>(users);
            _mockUserManager.Setup(_ => _.Users).Returns(asyncUserEnumerable.AsQueryable()).Verifiable();

            _administratorService = new AdministratorService(_mockLogger.Object, _mockRoleManager.Object, _mockUserManager.Object);
        }

        #region GetRolesAsync

        [Test]
        public async Task GetRolesAsync_ReturnsRoles_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.GetRolesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Any(), Is.True);
            Assert.That(result.Data.First().Permissions.Any(), Is.True);
        }

        [Test]
        public async Task GetRolesAsync_CallsRoleManager_WhenSuccessful()
        {
            // Arrange

            // Act
            await _administratorService.GetRolesAsync();

            // Assert
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.AtLeast(2));
        }

        [Test]
        public async Task GetRolesAsync_ReturnsNoRoles_WhenNoRoles()
        {
            // Arrange
            var roles = new List<IdentityRole>();
            var asyncRolesEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncRolesEnumerable.AsQueryable()).Verifiable();

            // Act
            var result = await _administratorService.GetRolesAsync();

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        #endregion GetRolesAsync

        #region GetPermissionsAsync

        [Test]
        public async Task GetPermissionsAsync_ReturnsPermissions_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.GetPermissionsAsync();

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Count(), Is.GreaterThan(0));
        }

        #endregion GetPermissionsAsync

        #region UpdateRole

        [Test]
        public async Task UpdateRole_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var role = new RoleViewModel()
            {
                Id = "UPDATEROLE",
                Name = "UpdateRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Create
                }
            };

            // Act
            var result = await _administratorService.UpdateRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task UpdateRole_AddsRemoves_WhenSuccessful()
        {
            // Arrange
            var role = new RoleViewModel()
            {
                Id = "UPDATEROLE",
                Name = "UpdateRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Delete
                }
            };

            // Act
            var result = await _administratorService.UpdateRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
            _mockRoleManager.Verify(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
        }

        [Test]
        public async Task UpdateRole_ReturnsFailed_WhenNoRoleExists()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole)null).Verifiable();
            var role = new RoleViewModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Create
                }
            };

            // Act
            var result = await _administratorService.UpdateRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateRole_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.UpdateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Error", Code = "Error" })).Verifiable();
            var role = new RoleViewModel()
            {
                Id = "UPDATEROLE",
                Name = "UpdateRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Create
                }
            };

            // Act
            var result = await _administratorService.UpdateRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion UpdateRole

        #region AddRoleAsync

        [Test]
        public async Task AddRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            var role = new RoleViewModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task AddRoleAsync_AddExpectedPermissions_WhenSuccessful()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            var role = new RoleViewModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.View
                }
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Data.Permissions.Any(), Is.True);
            _mockRoleManager.Verify(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Test]
        public async Task AddRoleAsync_ReturnsFailure_WhenExistingRole()
        {
            // Arrange
            var role = new RoleViewModel()
            {
                Name = DefaultRoles.User,
                Permissions = new List<string>()
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task AddRoleAsync_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            _mockRoleManager.Setup(_ => _.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();
            var role = new RoleViewModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
            _mockRoleManager.Verify(_ => _.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Test]
        public async Task AddRoleAsync_ReturnsFailure_WhenIdentityErrorOnPermissions()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();
            _mockRoleManager.Setup(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();
            var role = new RoleViewModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Edit
                }
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
            _mockRoleManager.Verify(_ => _.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        #endregion AddRoleAsync

        #region RemoveRoleAsync

        [Test]
        public async Task RemoveRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemoveRoleAsync("RemoveRole");

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task RemoveRoleAsync_CallsRoleManagerAsExpected_WhenSuccessful()
        {
            // Arrange
            const string name = "RemoveRole";

            // Act
            var result = await _administratorService.RemoveRoleAsync(name);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.DeleteAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Test]
        public async Task RemoveRoleAsync_ReturnsFailure_WhenNoExistingRole()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole)null).Verifiable();

            // Act
            var result = await _administratorService.RemoveRoleAsync("TestRole");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.DeleteAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Test]
        public async Task RemoveRoleAsync_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();

            // Act
            var result = await _administratorService.RemoveRoleAsync("RemoveRole");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.DeleteAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        #endregion RemoveRoleAsync

        #region GetUsersAsync

        [Test]
        public async Task GetUsersAsync_ReturnsUsers_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.GetUsersAsync();

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Count(), Is.GreaterThan(0));
        }

        [Test]
        public async Task GetUsersAsync_CallsUserManagerAtLeastThreeTimes_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.GetUsersAsync();

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockUserManager.Verify(_ => _.Users, Times.Once);
            _mockUserManager.Verify(_ => _.GetRolesAsync(It.IsAny<ApplicationUser>()), Times.Once);
            _mockUserManager.Verify(_ => _.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        public async Task GetUsersAsync_ReturnsExpectedProfiles_WhenSuccessful()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>() { DefaultRoles.User });

            // Act
            var result = await _administratorService.GetUsersAsync();

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Any(), Is.True);
            Assert.That(result.Data.First().Roles.First().Permissions.Any(), Is.True);
        }

        #endregion GetUsersAsync

        #region UpdateUserAsync

        [Test]
        public async Task UpdateUserAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var profile = new ProfileModel()
            {
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsFailure_WhenNoUser()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null)
                .Verifiable();
            var profile = new ProfileModel()
            {
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsUpdatedProfile_WhenEveryPropertyUpdated()
        {
            // Arrange
            var profile = new ProfileModel()
            {
                Email = "testemail",
                UserName = "testuser",
                PhoneNumber = "18008888888",
                FirstName = "Test",
                LastName = "User",
                OfficeLocation = new OfficeLocation()
                {
                    Id = 2,
                    Name = "Office 2"
                },
                Title = "Title"
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data.Email, Is.EqualTo(profile.Email));
            Assert.That(result.Data.UserName, Is.EqualTo(profile.UserName));
            Assert.That(result.Data.PhoneNumber, Is.EqualTo(profile.PhoneNumber));
            Assert.That(result.Data.FirstName, Is.EqualTo(profile.FirstName));
            Assert.That(result.Data.LastName, Is.EqualTo(profile.LastName));
            Assert.That(result.Data.OfficeLocation, Is.EqualTo(profile.OfficeLocation));
            Assert.That(result.Data.Title, Is.EqualTo(profile.Title));
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Error", Code = "Error" })).Verifiable();
            var profile = new ProfileModel()
            {
                Email = "testemail",
                UserName = "testuser",
                PhoneNumber = "18008888888",
                FirstName = "Test",
                LastName = "User",
                OfficeLocation = new OfficeLocation()
                {
                    Id = 2,
                    Name = "Office 2"
                },
                Title = "Title"
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsUpdatedProfile_WhenPermissionsUpdated()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.GetClaimsAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<Claim>() { new Claim(CustomClaimTypes.Permission, Permissions.Users.Delete) })
                .Verifiable();
            var profile = new ProfileModel()
            {
                Permissions = new[] { Permissions.Users.View }
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockUserManager.Verify(_ => _.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<Claim>>(claims => claims.FirstOrDefault(c => c.Value.Equals(Permissions.Users.View)) != null)), Times.Once);
            _mockUserManager.Verify(_ => _.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<Claim>>(claims => claims.FirstOrDefault(c => c.Value.Equals(Permissions.Users.Delete)) != null)), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsFailure_WhenPermissionsIdentityError()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Error", Code = "Error" })).Verifiable();
            var profile = new ProfileModel()
            {
                Permissions = new[] { Permissions.Users.View }
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<Claim>>(claims => claims.FirstOrDefault(c => c.Value.Equals(Permissions.Users.View)) != null)), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsUpdatedProfile_WhenRolesUpdated()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>() { "WillRemoveRole" })
                .Verifiable();
            var profile = new ProfileModel()
            {
                Roles = new[] {
                    new RoleViewModel()
                    {
                        Name = "Role",
                        Id = "Role"
                    }
                }
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockUserManager.Verify(_ => _.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<string>>(claims => claims.FirstOrDefault(c => c.Equals("Role")) != null)), Times.Once);
            _mockUserManager.Verify(_ => _.RemoveFromRolesAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<string>>(claims => claims.FirstOrDefault(c => c.Equals("WillRemoveRole")) != null)), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ReturnsFailure_WhenRolesIdentityError()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() {Description = "Error", Code = "Error"})).Verifiable();
            var profile = new ProfileModel()
            {
                Roles = new[] {
                    new RoleViewModel()
                    {
                        Name = "Role",
                        Id = "Role"
                    }
                }
            };

            // Act
            var result = await _administratorService.UpdateUserAsync("TestUser", profile);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.Is<IEnumerable<string>>(claims => claims.FirstOrDefault(c => c.Equals("Role")) != null)), Times.Once);
        }

        #endregion UpdateUserAsync

    }
}