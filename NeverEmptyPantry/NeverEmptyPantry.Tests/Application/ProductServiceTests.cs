using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;
using NUnit.Framework;
using MockFactory = NeverEmptyPantry.Tests.Util.MockFactory;

namespace NeverEmptyPantry.Tests.Application
{
    public class ProductServiceTests
    {
        private Mock<IRepository<Product>> _mockProductRepository;
        private Mock<ILogger<IProductService>> _mockLogger;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _mockProductRepository = MockFactory.GetMockRepository<Product>();
            _mockLogger = MockFactory.GetMockLogger<IProductService>();

            _productService = new ProductService(_mockProductRepository.Object, _mockLogger.Object);
        }

        #region CreateAsync

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

        #endregion ReadAsync

        #region UpdateAsync

        #endregion UpdateAsync

        #region RemoveAsync

        #endregion RemoveAsync
    }
}