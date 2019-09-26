using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NUnit.Framework;
using MockFactory = NeverEmptyPantry.Tests.Util.MockFactory;

namespace NeverEmptyPantry.Tests.Application
{
    [ExcludeFromCodeCoverage]
    public class ProductServiceTests
    {
        private Mock<IRepository<Product>> _mockProductRepository;
        private Mock<ILogger<IProductService>> _mockLogger;
        private Mock<IAuthenticationService> _mockAuthenticationService;
        private ProductService _productService;
        private Mock<IValidatorFactory<Product>> _mockValidatorFactory;
        private Mock<IValidator<Product>> _mockValidator;

        [SetUp]
        public void SetUp()
        {
            _mockProductRepository = MockFactory.GetMockRepository<Product>();
            _mockLogger = MockFactory.GetMockLogger<IProductService>();
            _mockAuthenticationService = MockFactory.GetMockAuthenticationService();
            _mockValidator = MockFactory.GetMockValidator<Product>();
            _mockValidatorFactory = MockFactory.GetMockValidatorFactory(_mockValidator);

            _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object, _mockAuthenticationService.Object, _mockValidatorFactory.Object);
        }

        #region CreateAsync

        [Test]
        public void CreateAsync_ThrowsException_WhenArgumentNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _productService.CreateAsync(null));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public async Task CreateAsync_ReturnsFailed_WhenRepoFails()
        {
            // Arrange
            _mockProductRepository.Setup(_ => _.CreateAsync(It.IsAny<Product>(), It.IsAny<string>()))
                .Throws<Exception>();
            var model = new Product()
            {
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.CreateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task CreateAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var model = new Product() {
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.CreateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task CreateAsync_ReturnsFailed_WhenNotValid()
        {
            // Arrange
            _mockValidator.Setup(_ => _.Validate(It.IsAny<Product>())).Returns(OperationResult.Failed());
            var model = new Product()
            {
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.CreateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion CreateAsync

        #region ReadAsync

        [Test]
        public void ReadAsync_ThrowsException_WhenArgumentNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _productService.ReadAsync(null));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public async Task ReadAsync_ReturnsExpectedResult_WhenSuccessful()
        {
            // Arrange
            var product = new Product();
            _mockProductRepository.Setup(_ => _.ReadAsync(It.IsAny<Func<Product, bool>>())).ReturnsAsync(new List<Product>() { product });

            // Act
            var result = await _productService.ReadAsync(e => e.Id == 1);

            // Assert
            Assert.That(result.Succeeded, Is.True);
            Assert.That(result.Data[0], Is.EqualTo(product));
        }

        [Test]
        public async Task ReadAsync_ReturnsFailed_WhenRepoFails()
        {
            // Arrange
            var product = new Product();
            _mockProductRepository.Setup(_ => _.ReadAsync(It.IsAny<Func<Product, bool>>())).Throws(new Exception("Error"));

            // Act
            var result = await _productService.ReadAsync(e => e.Id == 1);

            // Assert
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        }

        #endregion ReadAsync

        #region UpdateAsync

        [Test]
        public void UpdateAsync_ThrowsException_WhenArgumentNull()
        {
            // Arrange

            // Act
            var asyncTestDelegate = new AsyncTestDelegate(async () => await _productService.UpdateAsync(null));

            // Assert
            Assert.That(asyncTestDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public async Task UpdateAsync_ReturnsFailed_WhenRepoFails()
        {
            // Arrange
            _mockProductRepository.Setup(_ => _.UpdateAsync(It.IsAny<Product>(), It.IsAny<string>()))
                .Throws<Exception>();
            var model = new Product()
            {
                Id = 1,
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.UpdateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task UpdateAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange
            var model = new Product()
            {
                Id = 1,
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.UpdateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task UpdateAsync_ReturnsFailed_WhenNotValid()
        {
            // Arrange
            _mockValidator.Setup(_ => _.Validate(It.IsAny<Product>())).Returns(OperationResult.Failed());
            var model = new Product()
            {
                Id = 1,
                Name = "Test Product",
                Active = true,
                Brand = "Test",
                CreatedDateTimeUtc = DateTime.UtcNow,
                ModifiedDateTimeUtc = DateTime.UtcNow,
                PackSize = 5,
                UnitSize = "4"
            };

            // Act
            var result = await _productService.UpdateAsync(model);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        #endregion UpdateAsync

        #region RemoveAsync

        [Test]
        public async Task RemoveAsync_ReturnsFailed_WhenRepoFails()
        {
            // Arrange
            _mockProductRepository.Setup(_ => _.RemoveAsync(It.IsAny<Product>(), It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _productService.RemoveAsync(1);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task RemoveAsync_ReturnsFailed_WhenNotValid()
        {
            // Arrange
            _mockValidator.Setup(_ => _.Validate(It.IsAny<Product>())).Returns(OperationResult.Failed());

            // Act
            var result = await _productService.RemoveAsync(1);

            // Assert
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task RemoveAsync_ReturnsSuccess_WhenSuccessful()
        {
            // Arrange

            // Act
            var result = await _productService.RemoveAsync(1);

            // Assert
            Assert.That(result.Succeeded, Is.True);
        }

        #endregion RemoveAsync
    }
}