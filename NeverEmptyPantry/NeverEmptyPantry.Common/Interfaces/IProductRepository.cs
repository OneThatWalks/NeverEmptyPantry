using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductResult> AddProductAsync(ProductDto product);
        Task<ProductResult> UpdateProductAsync(int id, ProductDto product);
        Task<ProductResult> RemoveProductAsync(int id);
        Task<Product> GetProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> Products(Func<Product, bool> query);
    }
}