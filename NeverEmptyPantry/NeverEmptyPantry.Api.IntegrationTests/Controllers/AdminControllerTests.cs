using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Admin;
using NeverEmptyPantry.Repository.Entity;
using NUnit.Framework;

namespace NeverEmptyPantry.Api.IntegrationTests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class AdminControllerTests
    {
        private CustomWebApplicationFactory<IntegrationStartup> _factory;
        private HttpClient _client;


        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new CustomWebApplicationFactory<IntegrationStartup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = true
            });

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            SeedData.PopulateTestData(dbContext);
            await SeedData.SeedTestRolesAndPermissions(roleManager);
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

        #region Roles

        // GET: /api/admin/roles

        [Test]
        public async Task GETRoles_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/roles");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETRoles_ReturnsRoles_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<IEnumerable<RoleViewModel>>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
        }

        #endregion Roles

        #region Permissions

        // GET: /api/admin/permissions

        [Test]
        public async Task GETPermissions_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/permissions");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETPermissions_ReturnsPermissions_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/permissions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<IEnumerable<string>>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
            Assert.That(content.Data.Count(), Is.EqualTo(Permissions.All.Count()));
        }

        #endregion Permissions

        #region UpdateRole

        // PUT: /api/admin/roles

        [Test]
        public async Task PUTRoles_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            var model = new RoleViewModel
            {
                Id = "TestRole1",
                Name = "TestRole1",
                Permissions = new []
                {
                    Permissions.Users.Delete,
                    Permissions.Users.View
                }
            };
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/roles");
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task PUTRoles_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            var model = new
            {
                TestProperty = "TestRole1",
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PUTRoles_ReturnsOk_WhenModelValid()
        {
            // Arrange
            var model = new RoleViewModel
            {
                Id = "TestRole1",
                Name = "TestRole1",
                Permissions = new[]
                {
                    Permissions.Users.Delete,
                    Permissions.Users.View
                }
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion UpdateRole

        #region AddRole

        // POST: /api/admin/roles

        [Test]
        public async Task POSTRoles_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            var model = new RoleViewModel
            {
                Id = "TestRole3",
                Name = "TestRole3",
                Permissions = new[]
                {
                    Permissions.Users.Delete,
                    Permissions.Users.View
                }
            };
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/roles");
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task POSTRoles_ReturnsBadRequest_WhenModelInvalid()
        {
            // Arrange
            var model = new
            {
                TestProperty = "TestRole3",
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task POSTRoles_ReturnsOk_WhenModelValid()
        {
            // Arrange
            var model = new RoleViewModel
            {
                Id = "TestRole3",
                Name = "TestRole3",
                Permissions = new[]
                {
                    Permissions.Users.Delete,
                    Permissions.Users.View
                }
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/admin/roles");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion AddRole

        #region RemoveRole

        // DELETE: /api/admin/roles/RoleId

        [Test]
        public async Task DELETERoles_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/admin/roles/TestRole2");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DELETERoles_ReturnsBadRequest_WhenNoRole()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/admin/roles/test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task DELETERoles_ReturnsOk_WhenDeleted()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Delete, "/api/admin/roles/TestRole2");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion RemoveRole

        #region Users

        // GET: /api/admin/users

        [Test]
        public async Task GETUsers_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETUsers_ReturnsUsers_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<IEnumerable<ProfileModel>>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
            Assert.That(content.Data.Any(), Is.True);
        }

        #endregion Users

        #region UpdateUser

        // PUT: /api/admin/users/{userId}

        [Test]
        public async Task PUTUsers_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            var model = new ProfileModel
            {
                FirstName = "Re-test"
            };
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/users/TestUser1");
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task PUTUsers_ReturnsUsers_WhenSuccessful()
        {
            // Arrange
            var model = new ProfileModel
            {
                FirstName = "Re-test"
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/admin/users/TESTUSER1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = IntegrationHelpers.CreateHttpContent(model);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<ProfileModel>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
            Assert.That(content.Data.FirstName, Is.EqualTo(model.FirstName));
        }

        #endregion UpdateUser
    }
}