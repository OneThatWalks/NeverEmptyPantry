using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IListProductService
    {
        Task<ListProductsResult> GetListProducts(int listId);
        Task<ListProductResult> GetListProduct(int listId, int productId);
        Task<ListProductResult> GetOrCreateListProduct(int listId, int productId);
        Task<ListProductResult> CreateListProduct(int listId, int productId);
        Task<ListProductResult> RemoveListProduct(int listId, int productId);
        Task<ListProductResult> UpdateListProduct(int listId, int productId, ListProductState state);
    }
}