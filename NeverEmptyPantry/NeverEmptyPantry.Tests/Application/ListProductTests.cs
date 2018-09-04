using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Tests.Application
{
    [TestClass]
    public class ListProductTests
    {
        private MockRepository _mockRepository;
        private Mock<IListProductRepository> _mockListProductRepository;
        private Mock<IProductRepository> _mockProductRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock factory
            _mockRepository = new MockRepository(MockBehavior.Default);

            _mockListProductRepository = _mockRepository.Create<IListProductRepository>();
            _mockProductRepository = _mockRepository.Create<IProductRepository>();
        }

        [TestMethod]
        public async Task GetListProducts_ReturnsListProducts()
        {
            // Arrange
            var listProductDto = new ListProductDto
            {
                Id = 1
            };
            var listResult = new List<ListProductDto>
            {
                listProductDto
            };
            var retVal = ListProductsResult.ListProductsSuccess(listResult.ToArray());
            _mockListProductRepository.Setup(lr => lr.GetListProductsAsync(It.IsAny<int>())).ReturnsAsync(retVal);

            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.GetListProducts(1);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(listProductDto.Id, result.ListProducts.FirstOrDefault()?.Id);
        }

        [TestMethod]
        public async Task GetListProduct_ReturnsListProduct()
        {
            // Arrange
            var listProductDto = new ListProductDto
            {
                Id = 1
            };
            var listProdResult = ListProductResult.ListProductSuccess(listProductDto, null);
            _mockListProductRepository.Setup(lr => lr.GetListProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(listProdResult);

            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.GetListProduct(1, 1);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(listProductDto.Id, result.ListProduct.Id);
        }

        [TestMethod]
        public async Task CreateListProduct_ReturnsListProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1
            };
            _mockProductRepository.Setup(pr => pr.GetProductAsync(It.IsAny<int>())).ReturnsAsync(product);
            var listProduct = new ListProductDto
            {
                Id = 1
            };
            var listProductResult = ListProductResult.ListProductSuccess(listProduct, null);
            _mockListProductRepository.Setup(lr => lr.AddListProductAsync(It.IsAny<int>(), It.IsAny<Product>()))
                .ReturnsAsync(listProductResult);

            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.CreateListProduct(1, 1);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(listProduct.Id, result.ListProduct.Id);
        }

        [TestMethod]
        public async Task CreateListProduct_ReturnsFailedWithNoProduct()
        {
            // Arrange
            Product product = null;
            _mockProductRepository.Setup(pr => pr.GetProductAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.CreateListProduct(1, 1);

            // Assert
            Assert.IsTrue(!result.Succeeded);
            Assert.AreEqual(ErrorCodes.EntityFrameworkNotFoundError, result.Errors.FirstOrDefault()?.Code);
        }

        [TestMethod]
        public async Task RemoveListProduct_ReturnsSuccess()
        {
            // Arrange
            var listRemoveResult = ListProductResult.ListProductSuccess(null, null);
            _mockListProductRepository.Setup(lr => lr.RemoveListProductAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(listRemoveResult);

            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.RemoveListProduct(1, 1);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [TestMethod]
        public async Task UpdateListProduct_ReturnsListProduct()
        {
            // Arrange
            var listProduct = new ListProductDto
            {
                Id = 1,
                ListProductState = ListProductState.ITEM_REJECTED
            };
            var listRemoveResult = ListProductResult.ListProductSuccess(listProduct, null);
            _mockListProductRepository.Setup(lr => lr.UpdateListProductStateAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<ListProductState>())).ReturnsAsync(listRemoveResult);
            
            // Act
            var service = new ListProductService(_mockListProductRepository.Object, _mockProductRepository.Object);
            var result = await service.UpdateListProduct(1, 1, ListProductState.ITEM_REJECTED);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(ListProductState.ITEM_REJECTED, result.ListProduct.ListProductState);
        }
    }
}