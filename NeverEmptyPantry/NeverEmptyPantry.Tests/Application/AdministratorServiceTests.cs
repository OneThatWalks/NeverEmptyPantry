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

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<IAdministratorService>>();
            _mockRoleManager = MockFactory.MockRoleManager<IdentityRole>();

            var roles = new List<IdentityRole>()
                {new IdentityRole(DefaultRoles.Administrator) {Id = DefaultRoles.Administrator.ToUpper()}, new IdentityRole(DefaultRoles.User) {Id = DefaultRoles.User.ToUpper()}};
            var asyncEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncEnumerable.AsQueryable()).Verifiable();
            _mockRoleManager.Setup(_ => _.GetClaimsAsync(It.IsAny<IdentityRole>())).ReturnsAsync(new List<Claim>() { new Claim(CustomClaimTypes.Permission, Permissions.Users.Create) }).Verifiable();
            _mockRoleManager.Setup(_ => _.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();
            _mockRoleManager.Setup(_ => _.RemoveClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success).Verifiable();

            _administratorService = new AdministratorService(_mockLogger.Object, _mockRoleManager.Object);
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
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View });

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task AddPermissionsToRoleAsync_CallsRoleManagerAtLeastThreeSeparateTimes_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View });

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
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View });

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
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View });

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
            var result = await _administratorService.AddPermissionsToRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View, Permissions.Users.Create });

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
                await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator.ToUpper(),
                    new[] { Permissions.Users.Create });

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task RemovePermissionsFromRoleAsync_CallsRoleManagerAtLeastThreeSeparateTimes_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.Create });

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
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.Create });

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
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.Create });

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
            var result = await _administratorService.RemovePermissionsFromRoleAsync(DefaultRoles.Administrator.ToUpper(), new[] { Permissions.Users.View, Permissions.Users.Create });

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

        //TODO: Finish unit testing admin service, then integration test

        #endregion AddRoleAsync
    }
}