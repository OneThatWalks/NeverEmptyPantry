using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Common.Models;
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
        public void OneTimeSetUp()
        {
            _factory = new CustomWebApplicationFactory<IntegrationStartup>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions()
            {
                AllowAutoRedirect = true
            });

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            SeedData.PopulateTestData(dbContext);
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

        #endregion Permissions

        #region UpdateRole

        // PUT: /api/admin/roles

        #endregion UpdateRole

        #region AddRole

        // POST: /api/admin/roles

        #endregion AddRole

        #region RemoveRole

        // DELETE: /api/admin/roles/RoleId

        #endregion RemoveRole

        #region Users

        // GET: /api/admin/users

        #endregion Users

        #region UpdateUser

        // PUT: /api/admin/users

        #endregion UpdateUser
    }
}