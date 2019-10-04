using System;
using Microsoft.AspNetCore.Mvc.Testing;
using NeverEmptyPantry.Api.IntegrationTests.Util;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NeverEmptyPantry.Repository.Entity;

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

        #region Get

        // GET: /api/product

        [Test]
        public async Task GETProduct_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/product");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETProduct_ReturnsProduct_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/product");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<IList<Product>>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
        }

        // GET: /api/product/1

        [Test]
        public async Task GETProductById_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/1");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GETProductById_ReturnsProduct_WhenSuccessful()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<Product>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Data, Is.Not.Null);
        }

        [Test]
        public async Task GETProductById_ReturnsNotFound_WhenSuccessfulNoProduct()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/product/0");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        #endregion Get

        #region Put

        // PUT: /api/products/1

        [Test]
        public async Task PUTProductById_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/product/1");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task PUTProductById_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);

            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/product/1")
            {
                Content = IntegrationHelpers.CreateHttpContent(new Product()),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PUTProductById_ReturnsOk_WhenProductUpdated()
        {
            // Arrange
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            var product = await IntegrationHelpers.GetProductAsync(_client, 1);
            product.Brand = "Brand 2";

            using var request = new HttpRequestMessage(HttpMethod.Put, "/api/product/1")
            {
                Content = IntegrationHelpers.CreateHttpContent(product),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<Product>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.Data.Brand, Is.EqualTo("Brand 2"));
        }

        #endregion Put

        #region Post

        // POST: /api/products/

        [Test]
        public async Task POSTProduct_ReturnsUnauth_WhenNotAuthorized()
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/product/");

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task POSTProduct_ReturnsBadRequest_WhenNotValid()
        {
            // Arrange
            var product = new Product();
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/product/")
            {
                Content = IntegrationHelpers.CreateHttpContent(product)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task POSTProduct_ReturnsOk_WhenCreated()
        {
            // Arrange
            var product = new Product()
            {
                Name = "Testing Product",
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow
            };
            var token = await IntegrationHelpers.GetAuthorizationTokenAsync(_client);
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/product/")
            {
                Content = IntegrationHelpers.CreateHttpContent(product)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            using var response = await _client.SendAsync(request);
            var content = await
                IntegrationHelpers.DeserializeHttpContentAsync<OperationResult<Product>>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content.Data.Name, Is.EqualTo("Testing Product"));
        }

        #endregion Post
    }
}