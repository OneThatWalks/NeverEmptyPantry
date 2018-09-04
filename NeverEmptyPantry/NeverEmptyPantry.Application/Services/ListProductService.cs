using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Application.Services
{
    public class ListProductService : IListProductService
    {
        private readonly IListProductRepository _listProductRepository;
        private readonly IProductRepository _productRepository;

        public ListProductService(IListProductRepository listProductRepository, IProductRepository productRepository)
        {
            _listProductRepository = listProductRepository;
            _productRepository = productRepository;
        }
        public async Task<ListProductsResult> GetListProducts(int listId)
        {
            var listProducts = await _listProductRepository.GetListProductsAsync(listId);

            return listProducts;
        }

        public async Task<ListProductResult> GetListProduct(int listId, int productId)
        {
            var listProductResult = await _listProductRepository.GetListProductAsync(listId, productId);

            return listProductResult;
        }

        public async Task<ListProductResult> GetOrCreateListProduct(int listId, int productId)
        {
            var listProductResult = await _listProductRepository.GetListProductAsync(listId, productId);

            if (!listProductResult.Succeeded || listProductResult.ListProduct == null)
            {
                listProductResult = await CreateListProduct(listId, productId);
            }

            return listProductResult;
        }

        public async Task<ListProductResult> CreateListProduct(int listId, int productId)
        {
            var product = await _productRepository.GetProductAsync(productId);

            if (product == null)
            {
                return ListProductResult.ListProductFailed(new Error
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Product with id {productId} not found."
                });
            }

            var createListProductResult = await _listProductRepository.AddListProductAsync(listId, product);

            return createListProductResult;
        }

        public async Task<ListProductResult> RemoveListProduct(int listId, int productId)
        {
            var removeProductResult = await _listProductRepository.RemoveListProductAsync(listId, productId);

            return removeProductResult;
        }

        public async Task<ListProductResult> UpdateListProduct(int listId, int productId, ListProductState state)
        {
            var updateListProductResult = await _listProductRepository.UpdateListProductStateAsync(listId, productId, state);

            return updateListProductResult;
        }
    }
}