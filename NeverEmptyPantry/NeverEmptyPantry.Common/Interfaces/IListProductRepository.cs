using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IListProductRepository
    {
        Task<ListProductsResult> GetListProductsAsync(int listId);
        Task<ListProductResult> GetListProductAsync(int listId, int productId);
        Task<ListProductResult> GetListProductAsync(int listProductId);
        Task<ListProductResult> AddListProductAsync(int listId, Product productId);
        Task<ListProductResult> RemoveListProductAsync(int listId, int productId);
        Task<ListProductResult> UpdateListProductStateAsync(int listId, int productId, ListProductState state);
    }
}