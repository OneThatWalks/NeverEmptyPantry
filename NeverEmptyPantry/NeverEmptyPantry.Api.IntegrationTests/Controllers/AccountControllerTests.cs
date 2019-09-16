using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using Newtonsoft.Json;
using NUnit.Framework;

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
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Authenticate_ReturnsUnauthorized_WhenLoginInvalid()
        {
            // Arrange
            var model = new LoginModel
            {
                Username = "Test",
                Password = "Test"
            };

            // Act
            var result = await _client.PostAsync("/api/account/authenticate", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            // Assert
            Assert.That(result.StatusCode == HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task Authenticate_ReturnsOk_WhenLoginValid()
        {
            // Arrange
            var model = new LoginModel
            {
                Username = "TestUser1",
                Password = "Str0ngP@ssword"
            };

            // Act
            var result = await _client.PostAsync("/api/account/authenticate", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

            var resultContent = await result.Content.ReadAsStringAsync();

            var resultObject = JsonConvert.DeserializeObject<OperationResult<TokenModel>>(resultContent);

            // Assert
            Assert.That(result.StatusCode == HttpStatusCode.OK);
            Assert.That(resultObject, Is.Not.Null);
            Assert.That(resultObject.Data.Token, Is.Not.Null);
        }
    }
}