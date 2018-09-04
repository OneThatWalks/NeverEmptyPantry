using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IProductService
    {
        Task<ProductsResult> GetProducts();
        Task<ProductResult> GetProduct(int productId);
        Task<ProductResult> AddProduct(ProductDto model);
        Task<ProductResult> RemoveProduct(int productId);
        Task<ProductResult> UpdateProduct(int productId, ProductDto model);

    }
}