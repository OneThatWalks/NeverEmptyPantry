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
using System.Threading.Tasks;
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

        // TODO: Might want to analyze error handling somewhere in the service

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
            var role = new RoleModel()
            {
                Name = "UpdateRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Create
                }
            };

            // Act
            var result = await _administratorService.UpdateRole(role);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task UpdateRole_ReturnsFailed_WhenNoRoleExists()
        {
            // Arrange
            var role = new RoleModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
                {
                    Permissions.Users.Create
                }
            };

            // Act
            var result = await _administratorService.UpdateRole(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion UpdateRole

        #region AddRoleAsync

        [Test]
        public async Task AddRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var role = new RoleModel()
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
        public async Task AddRoleAsync_ReturnsFailure_WhenExistingRole()
        {
            // Arrange
            var role = new RoleModel()
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
        public async Task AddRoleAsync_ReturnsFailure_WheIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();
            var role = new RoleModel()
            {
                Name = "TestRole",
                Permissions = new List<string>()
            };

            // Act
            var result = await _administratorService.AddRoleAsync(role);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(1));
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
            _mockRoleManager.Verify(_ => _.CreateAsync(It.IsAny<IdentityRole>()), Times.Once);
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
        public async Task RemoveRoleAsync_CallsRoleManagerFourTimesWithExpectedParameters_WhenSuccessful()
        {
            // Arrange
            const string name = "RemoveRole";

            // Act
            var result = await _administratorService.RemoveRoleAsync(name);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(1));
            _mockRoleManager.Verify(_ => _.DeleteAsync(It.Is<IdentityRole>(r => r.Name.Equals(name))), Times.Once);
        }

        [Test]
        public async Task RemoveRoleAsync_ReturnsFailure_WhenNoExistingRole()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemoveRoleAsync("TestRole");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(1));
            _mockRoleManager.Verify(_ => _.DeleteAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Test]
        public async Task RemoveRoleAsync_ReturnsFailure_WheIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.DeleteAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();

            // Act
            var result = await _administratorService.RemoveRoleAsync("RemoveRole");

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(1));
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

        #endregion GetUsersAsync

        
    }
}