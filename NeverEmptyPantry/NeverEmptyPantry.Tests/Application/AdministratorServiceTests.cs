using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Tests.Util;
using NUnit.Framework;
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
                new IdentityRole("RemoveRole") {Id = "RemoveRole".ToUpper()}
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

        #region AddPermissionsToRoleAsync

        [Test]
        public async Task AddPermissionsToRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View });

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task AddPermissionsToRoleAsync_CallsRoleManagerAtLeastThreeSeparateTimes_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View });

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task AddPermissionsToRoleAsync_ReturnsFailed_WhenNoRole()
        {
            // Arrange
            var roles = new List<IdentityRole>()
            { };
            var asyncEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncEnumerable.AsQueryable()).Verifiable();

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View });

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
        }

        [Test]
        public async Task AddPermissionsToRoleAsync_ReturnsFailed_WhenIdentityResultFailed()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Description = "", Code = "" } })).Verifiable();

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View });

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Test]
        public async Task AddPermissionsToRoleAsync_IgnoresExistingClaims_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View, Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
        }

        #endregion AddPermissionsToRoleAsync

        #region RemovePermissionsFromRoleAsync

        [Test]
        public async Task RemovePermissionsFromRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result =
                await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator,
                    new[] { Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task RemovePermissionsFromRoleAsync_CallsRoleManagerAtLeastThreeSeparateTimes_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task RemovePermissionsFromRoleAsync_ReturnsFailed_WhenNoRole()
        {
            // Arrange
            var roles = new List<IdentityRole>()
            { };
            var asyncEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncEnumerable.AsQueryable()).Verifiable();

            // Act
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
        }

        [Test]
        public async Task RemovePermissionsFromRoleAsync_ReturnsFailed_WhenIdentityResultFailed()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError() { Description = "", Code = "" } })).Verifiable();

            // Act
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
        }

        [Test]
        public async Task RemovePermissionsFromRoleAsync_IgnoresExistingClaims_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator, new[] { Permissions.Users.View, Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Once);
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Once);
        }

        #endregion RemovePermissionsFromRoleAsync

        #region AddRoleAsync

        [Test]
        public async Task AddRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddRoleAsync("TestRole");

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task AddRoleAsync_CallsRoleManagerFourTimesWithExpectedParameters_WhenSuccessful()
        {
            // Arrange
            const string name = "TestRole";

            // Act
            var result = await _administratorService.AddRoleAsync(name);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(2));
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Once);
            _mockRoleManager.Verify(_ => _.CreateAsync(It.Is<IdentityRole>(r => r.Name.Equals(name))), Times.Once);
        }

        [Test]
        public async Task AddRoleAsync_ReturnsFailure_WhenExistingRole()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddRoleAsync(DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.Roles, Times.Exactly(1));
            _mockRoleManager.Verify(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>()), Times.Never);
            _mockRoleManager.Verify(_ => _.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Test]
        public async Task AddRoleAsync_ReturnsFailure_WheIdentityError()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "Test", Code = "Test" })).Verifiable();

            // Act
            var result = await _administratorService.AddRoleAsync("TestRole");

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

        #region AddUserToRoleAsync

        [Test]
        public async Task AddUserToRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddUserToRoleAsync("TestUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task AddUserToRoleAsync_ReturnsFailure_WhenNoUser()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddUserToRoleAsync("NonexistentUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AddUserToRoleAsync_ReturnsFailure_WhenNoRole()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _administratorService.AddUserToRoleAsync("NonexistentUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.RoleExistsAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(_ => _.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AddUserToRoleAsync_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();

            // Act
            var result = await _administratorService.AddUserToRoleAsync("TestUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        #endregion AddUserToRoleAsync

        #region RemoveUserFromRoleAsync

        [Test]
        public async Task RemoveUserFromRoleAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemoveUserFromRoleAsync("TestUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task RemoveUserFromRoleAsync_ReturnsFailure_WhenNoUser()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemoveUserFromRoleAsync("NonexistentUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RemoveUserFromRoleAsync_ReturnsFailure_WhenNoRole()
        {
            // Arrange
            _mockRoleManager.Setup(_ => _.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _administratorService.RemoveUserFromRoleAsync("TestUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockRoleManager.Verify(_ => _.RoleExistsAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(_ => _.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RemoveUserFromRoleAsync_ReturnsFailure_WhenIdentityError()
        {
            // Arrange
            _mockUserManager.Setup(_ => _.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed()).Verifiable();

            // Act
            var result = await _administratorService.RemoveUserFromRoleAsync("TestUser", DefaultRoles.User);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            _mockUserManager.Verify(_ => _.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        #endregion RemoveUserFromRoleAsync
    }
}