using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeverEmptyPantry.Application.Services;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Vote;

namespace NeverEmptyPantry.Tests.Application
{
    [TestClass]
    public class ListTests
    {
        private MockRepository _mockRepository;
        private Mock<IListRepository> _mockListRepository;
        private Mock<IListProductRepository> _mockListProductRepository;
        private Mock<IUserVoteRepository> _mockUserVoteRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock factory
            _mockRepository = new MockRepository(MockBehavior.Default);

            _mockListRepository = _mockRepository.Create<IListRepository>();
            _mockListProductRepository = _mockRepository.Create<IListProductRepository>();
            _mockUserVoteRepository = _mockRepository.Create<IUserVoteRepository>();
        }

        [TestMethod]
        public async Task GetLists_ReturnsLists()
        {
            // Arrange
            var lists = new List<List>
            {
                new List
                {
                    Name = "ListyListFace"
                }
            };
            _mockListRepository.Setup(lr => lr.GetListsAsync()).ReturnsAsync(lists);

            // Act
            var service = new ListService(_mockListRepository.Object, _mockListProductRepository.Object, _mockUserVoteRepository.Object);
            var result = await service.GetLists();

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(1, result.Lists.Count());
            Assert.AreEqual(lists.FirstOrDefault()?.Name, result.Lists.FirstOrDefault()?.Name);
        }

        [TestMethod]
        public async Task GetList_ReturnsList()
        {
            // Arrange
            var list = new List
            {
                Name = "ListyMcListFace"
            };
            _mockListRepository.Setup(lr => lr.GetListAsync(It.IsAny<int>())).ReturnsAsync(list);


            var listProducts = new ListProductsResult
            {
                Succeeded = true,
                ListProducts = new[] {
                    new ListProductDto
                    {
                        Id = 1
                    }
                }
            };
            _mockListProductRepository.Setup(lpr => lpr.GetListProductsAsync(It.IsAny<int>()))
                .ReturnsAsync(listProducts);

            var votes = new List<UserProductVoteDto>
            {
                new UserProductVoteDto
                {
                    Id = 1
                }
            };
            _mockUserVoteRepository.Setup(uvr => uvr.GetListProductVotesAsync(It.IsAny<int>())).ReturnsAsync(votes);

            // Act
            var service = new ListService(_mockListRepository.Object, _mockListProductRepository.Object, _mockUserVoteRepository.Object);
            var result = await service.GetList(1);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(list.Name, result.List.Name);
            Assert.AreEqual(1, result.UserProductVotes.FirstOrDefault()?.Id);
            Assert.AreEqual(1, result.ListProducts.FirstOrDefault()?.Id);
        }

        [TestMethod]
        public async Task CreateList_ReturnsList()
        {
            // Arrange
            ListDto list = new ListDto
            {
                Name = "ListyMcListFace"
            };
            var repoResult = ListResult.ListSuccess(list, null, null);
            _mockListRepository.Setup(lr => lr.AddListAsync(It.IsAny<ListDto>())).ReturnsAsync(repoResult);

            // Act
            var service = new ListService(_mockListRepository.Object, _mockListProductRepository.Object, _mockUserVoteRepository.Object);
            var result = await service.CreateList(list);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(list.Name, result.List.Name);
        }

        [TestMethod]
        public async Task UpdateList_ReturnsList()
        {
            // Arrange
            ListDto list = new ListDto
            {
                Id = 1,
                Name = "ListyMcListFace"
            };
            var repoResult = ListResult.ListSuccess(list, null, null);
            _mockListRepository.Setup(lr => lr.UpdateListAsync(It.IsAny<int>(), It.IsAny<ListDto>())).ReturnsAsync(repoResult);

            // Act
            var service = new ListService(_mockListRepository.Object, _mockListProductRepository.Object, _mockUserVoteRepository.Object);
            var result = await service.UpdateList(list);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(list.Name, result.List.Name);
        }

        [TestMethod]
        public async Task RemoveLists_ReturnsSuccess()
        {
            // Arrange
            ListDto list = new ListDto
            {
                Id = 1,
                Name = "ListyMcListFace"
            };
            var repoResult = ListResult.ListSuccess(list, null, null);
            _mockListRepository.Setup(lr => lr.DeleteListAsync(It.IsAny<int>())).ReturnsAsync(repoResult);

            // Act
            var service = new ListService(_mockListRepository.Object, _mockListProductRepository.Object, _mockUserVoteRepository.Object);
            var result = await service.RemoveList(1);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

    }
}