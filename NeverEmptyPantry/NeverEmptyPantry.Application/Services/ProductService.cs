using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductsResult> GetProducts()
        {
            var products = await _productRepository.GetProductsAsync();

            var returnProducts = products.Select(ProductDto.From);

            return ProductsResult.ProductsSuccess(returnProducts);
        }

        public async Task<ProductResult> GetProduct(int productId)
        {
            var product = await _productRepository.GetProductAsync(productId);

            var returnProductDto = ProductDto.From(product);

            return ProductResult.ProductSuccess(returnProductDto);
        }

        public async Task<ProductResult> AddProduct(ProductDto model)
        {
            var result = await _productRepository.AddProductAsync(model);

            return result;
        }

        public async Task<ProductResult> RemoveProduct(int productId)
        {
            var result = await _productRepository.RemoveProductAsync(productId);

            return result;
        }

        public async Task<ProductResult> UpdateProduct(int productId, ProductDto model)
        {
            var result = await _productRepository.UpdateProductAsync(productId, model);

            return result;
        }
    }
}
