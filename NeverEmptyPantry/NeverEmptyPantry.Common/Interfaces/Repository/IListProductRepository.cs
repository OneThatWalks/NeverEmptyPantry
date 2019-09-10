using NeverEmptyPantry.Common.Models.List;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IListProductRepository
    {
        /// <summary>
        /// Gets the products from a list
        /// </summary>
        /// <param name="listId">The list to query</param>
        /// <returns>A task result that represents the list of products</returns>
        Task<IEnumerable<ListProductMap>> GetListProductsAsync(int listId);

        /// <summary>
        /// Gets a product from a list
        /// </summary>
        /// <param name="listId">The list to query</param>
        /// <param name="productId">The product in the list</param>
        /// <returns>A task result that represents a product map</returns>
        Task<ListProductMap> GetListProductAsync(int listId, int productId);

        /// <summary>
        /// Adds a product to a list
        /// </summary>
        /// <param name="listProductMap">The list product map to add</param>
        /// <returns>A task result that represents the new list product map</returns>
        Task<ListProductMap> AddListProductAsync(ListProductMap listProductMap);

        /// <summary>
        /// Removes a product from a list
        /// </summary>
        /// <param name="listProductMap">The list product map to remove</param>
        /// <param name="hardDelete">Whether to remove the record or set inactive</param>
        /// <returns>A task result that represents the removed list product map</returns>
        Task<ListProductMap> RemoveListProductAsync(ListProductMap listProductMap, bool hardDelete = false);

        /// <summary>
        /// Updates a list product map record
        /// </summary>
        /// <param name="listProductMap">The list product map to update</param>
        /// <returns>A task result that represents the updated list product map</returns>
        Task<ListProductMap> UpdateListProductMap(ListProductMap listProductMap);
    }
}