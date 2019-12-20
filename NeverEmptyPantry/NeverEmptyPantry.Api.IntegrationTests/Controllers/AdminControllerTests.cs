using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Api.IntegrationTests.Util;
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