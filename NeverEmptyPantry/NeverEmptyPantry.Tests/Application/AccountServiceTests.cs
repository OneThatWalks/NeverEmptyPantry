using System.Threading.Tasks;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Models.Account;
using NUnit.Framework;

namespace NeverEmptyPantry.Tests.Application
{
    [TestFixture]
    public class AccountServiceTests
    {
        private AccountService _accountService;


        [SetUp]
        public void SetUp()
        {

            _accountService = new AccountService();
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
        public async Task RegisterAsync_ReturnsFailed_WhenValidationFails()
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
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task RegisterAsync_ReturnsFailed_WhenUserWithUserNameAlreadyExists()
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
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion RegisterAsync
    }
}