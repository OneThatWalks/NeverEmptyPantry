using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IProductRepository
    {
        /// <summary>
        /// Adds a product
        /// </summary>
        /// <param name="product">The product to add</param>
        /// <returns>A task result that represents the added product</returns>
        Task<Product> AddProductAsync(Product product);

        /// <summary>
        /// Updates a product
        /// </summary>
        /// <param name="product">The product to update</param>
        /// <returns>A task result that represents the updated product</returns>
        Task<Product> UpdateProductAsync(Product product);

        /// <summary>
        /// Removes a product
        /// </summary>
        /// <param name="product">The product to remove</param>
        /// <returns>A task result that represents the removed product</returns>
        Task<Product> RemoveProductAsync(Product product);

        /// <summary>
        /// Gets a product
        /// </summary>
        /// <param name="id">The id to query</param>
        /// <returns>A task result that represents the product found</returns>
        Task<Product> GetProductAsync(int id);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>A task result that represents the products</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Filters the products based on a query function
        /// </summary>
        /// <param name="query">The query function</param>
        /// <returns>A task result that represents the products</returns>
        Task<IEnumerable<Product>> Products(Func<Product, bool> query);
    }
}