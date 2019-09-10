using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Tests.Application
{
    [TestClass]
    public class ProductTests
    {
        private MockRepository _mockRepository;
        private Mock<IProductRepository> _mockProductRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock factory
            _mockRepository = new MockRepository(MockBehavior.Default);

            _mockProductRepository = _mockRepository.Create<IProductRepository>();
        }

        [TestMethod]
        public async Task GetProducts_ReturnsSuccess()
        {
            // Arrange
            var list = new List<Product>()
            {
                new Product()
                {
                    Name = "FranklyImATest"
                }
            };
            _mockProductRepository.Setup(pr => pr.GetProductsAsync()).ReturnsAsync(list);

            // Act
            var service = new ProductService(_mockProductRepository.Object);
            var result = await service.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("FranklyImATest", result.Products.FirstOrDefault()?.Name);
        }

        [TestMethod]
        public async Task GetProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new Product()
            {
                Id = 0,
                Name = "FranklyImATest"
            };
            _mockProductRepository.Setup(pr => pr.GetProductAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var service = new ProductService(_mockProductRepository.Object);
            var result = await service.GetProduct(product.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("FranklyImATest", result.Product.Name);
        }

        [TestMethod]
        public async Task AddProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductDto()
            {
                Name = "FranklyImATest"
            };
            var retVal = ProductResult.ProductSuccess(product);
            _mockProductRepository.Setup(pr => pr.AddProductAsync(It.IsAny<ProductDto>())).ReturnsAsync(retVal);

            // Act
            var service = new ProductService(_mockProductRepository.Object);
            var result = await service.AddProduct(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("FranklyImATest", result.Product.Name);
        }

        [TestMethod]
        public async Task RemoveProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductDto()
            {
                Name = "FranklyImATest"
            };
            var retVal = ProductResult.ProductSuccess(product);
            _mockProductRepository.Setup(pr => pr.RemoveProductAsync(It.IsAny<int>())).ReturnsAsync(retVal);

            // Act
            var service = new ProductService(_mockProductRepository.Object);
            var result = await service.RemoveProduct(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("FranklyImATest", result.Product.Name);
        }

        [TestMethod]
        public async Task UpdateProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductDto()
            {
                Name = "FranklyImATest"
            };

            var retVal = ProductResult.ProductSuccess(product);
            _mockProductRepository.Setup(pr => pr.UpdateProductAsync(It.IsAny<int>(), It.IsAny<ProductDto>())).ReturnsAsync(retVal);

            // Act
            var service = new ProductService(_mockProductRepository.Object);
            var result = await service.UpdateProduct(1, product);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual("FranklyImATest", result.Product.Name);
        }
    }
}