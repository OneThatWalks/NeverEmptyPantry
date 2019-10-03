using Microsoft.AspNetCore.Mvc.Testing;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Repository.Entity;

namespace NeverEmptyPantry.Api.IntegrationTests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<IntegrationStartup> _factory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new CustomWebApplicationFactory<IntegrationStartup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = true
            });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureDeleted();

            _client.Dispose();
            _factory.Dispose();
        }

        #region Authenticate
        // POST: /api/account/authenticate

        [Test]
        public async Task POSTAuthenticate_ReturnsUnauthorized_WhenLoginInvalid()
        {
            // Arrange
            var model = new LoginModel
            {
                Username = "Test",
                Password = "Test"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/authenticate")
            {
                Content = IntegrationHelpers.CreateHttpContent(model)
            };

            // Act
            using var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task POSTAuthenticate_ReturnsOk_WhenLoginValid()
        {
            // Arrange
            var model = new LoginModel
            {
                Username = "TestUser1",
                Password = "Str0ngP@ssword"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/authenticate")
            {
                Content = IntegrationHelpers.CreateHttpContent(model)
            };

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<TokenModel>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data.Token, Is.Not.Null);
        }

        #endregion Authenticate

        #region Register
        // POST: /api/account/register

        [Test]
        public async Task POSTRegister_ReturnsBadRequest_WhenRegisterModelNotValid()
        {
            // Arrange
            var model = new RegistrationModel
            {
                Username = "TestUser3",
                Email = "BadEmail",
                LastName = "Doe",
                FirstName = "Jack",
                Title = "User",
                PhoneNumber = "",
                Password = "Str0ngP@ssword"
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/register")
            {
                Content = IntegrationHelpers.CreateHttpContent(model)
            };

            // Act
            using var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task POSTRegister_ReturnsOk_WhenRegisterModelValid()
        {
            // Arrange
            var model = new RegistrationModel
            {
                Username = "TestUser3",
                Email = "TestUser3@email.com",
                LastName = "Doe",
                FirstName = "Jack",
                Title = "User",
                PhoneNumber = "",
                Password = "Str0ngP@ssword",
                OfficeLocationId = 1
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/register")
            {
                Content = IntegrationHelpers.CreateHttpContent(model)
            };

            // Act
            using var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion Register

        #region Profile
        // GET: /api/account/profile

        [Test]
        public async Task GETProfile_ReturnsUnauthorized_WhenNoAuthOnRequest()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/profile");
            // Act
            using var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETProfile_ReturnsOk_WhenAuthorized()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<ProfileModel>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data.UserName, Is.EqualTo("TestUser1"));
        }

        // POST: /api/account/profile

        [Test]
        public async Task PUTProfile_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            var model = new ProfileModel
            {
                UserName = "TestUser1",
                Email = "BadEmail",
                FirstName = "John",
                LastName = "Doe" 
            };

            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/account/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PUTProfile_ReturnsBadRequest_WhenModelValid()
        {
            // Arrange
            var model = new ProfileModel
            {
                UserName = "TestUser1",
                Email = "NewEmail@user.com",
                FirstName = "John",
                LastName = "Doe"
            };

            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/account/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<ProfileModel>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.Data.Email, Is.EqualTo(model.Email));
        }

        #endregion Profile

    }
}