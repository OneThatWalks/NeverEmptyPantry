using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        #region Authenticate
        // POST: /api/account/authenticate

        [Test]
        public async Task Authenticate_ReturnsUnauthorized_WhenLoginInvalid()
        {
            // Arrange
            var model = new LoginModel
            {
                Username = "Test",
                Password = "Test"
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/authenticate"))
            {
                request.Content = IntegrationHelpers.CreateHttpContent(model);

                // Act
                using (var response = await _client.SendAsync(request))
                {

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                }
            }
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

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/authenticate"))
            {
                request.Content = IntegrationHelpers.CreateHttpContent(model);

                // Act
                using (var response = await _client.SendAsync(request))
                {
                    var content = await
                        IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<TokenModel>>(response.Content);

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(content, Is.Not.Null);
                    Assert.That(content.Data.Token, Is.Not.Null);
                }
            }
        }

        #endregion Authenticate

        #region Register
        // POST: /api/account/register

        [Test]
        public async Task Register_ReturnsBadRequest_WhenRegisterModelNotValid()
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

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/register"))
            {
                request.Content = IntegrationHelpers.CreateHttpContent(model);

                // Act
                using (var response = await _client.SendAsync(request))
                {

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                }
            }
        }

        [Test]
        public async Task Register_ReturnsOk_WhenRegisterModelValid()
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

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/account/register"))
            {
                request.Content = IntegrationHelpers.CreateHttpContent(model);

                // Act
                using (var response = await _client.SendAsync(request))
                {

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }
            }
        }

        #endregion Register

        #region Profile
        // GET: /api/account/profile

        [Test]
        public async Task Profile_ReturnsUnauthorized_WhenNoAuthOnRequest()
        {
            // Arrange
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/profile"))
            {
                // Act
                using (var response = await _client.SendAsync(request))
                {
                    
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
                }
            }
        }

        [Test]
        public async Task Profile_ReturnsOk_WhenAuthorized()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/profile"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Act
                using (var response = await _client.SendAsync(request))
                {
                    var content = await
                        IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<ProfileModel>>(response.Content);

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(content, Is.Not.Null);
                    Assert.That(content.Data.UserName, Is.EqualTo("TestUser1"));
                }
            }
        }

        #endregion Profile
    }
}