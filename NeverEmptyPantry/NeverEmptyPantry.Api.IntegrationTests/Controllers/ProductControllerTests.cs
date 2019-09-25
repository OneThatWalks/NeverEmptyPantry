using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Entity;
using Newtonsoft.Json;
using NUnit.Framework;

namespace NeverEmptyPantry.Api.IntegrationTests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
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

        #region Get

        // GET: /api/product

        [Test]
        public async Task GETProduct_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/product"))
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
        public async Task GETProduct_ReturnsProduct_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/product"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Act
                using (var response = await _client.SendAsync(request))
                {
                    var content = await
                        IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<IList<Product>>>(response.Content);

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(content, Is.Not.Null);
                    Assert.That(content.Data, Is.Not.Null);
                }
            }
        }

        // GET: /api/product/1

        [Test]
        public async Task GETProductById_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/1"))
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
        public async Task GETProductById_ReturnsProduct_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/1"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Act
                using (var response = await _client.SendAsync(request))
                {
                    var content = await
                        IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<Product>>(response.Content);

                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(content, Is.Not.Null);
                    Assert.That(content.Data, Is.Not.Null);
                }
            }
        }

        [Test]
        public async Task GETProductById_ReturnsNotFound_WhenSuccessfulNoProduct()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/0"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Act
                using (var response = await _client.SendAsync(request))
                {
                    // Assert
                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                }
            }
        }

        #endregion Get
    }
}