using System.Collections.Generic;
using System.Linq;
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
                {new IdentityRole(DefaultRoles.Administrator), new IdentityRole(DefaultRoles.User)};
            var asyncEnumerable = new TestAsyncEnumerable<IdentityRole>(roles);
            _mockRoleManager.Setup(_ => _.Roles).Returns(asyncEnumerable.AsQueryable()).Verifiable();

            _administratorService = new AdministratorService(_mockLogger.Object, _mockRoleManager.Object);
        }

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
    }
}