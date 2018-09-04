using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Product;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Repository.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProductResult> AddProductAsync(ProductDto product)
        {
            var productToAdd = new Product
            {
                Name = product.Name,
                Active = true,
                AddedDateTime = DateTime.UtcNow,
                Brand = product.Brand,
                Category = product.Category,
                Image = product.Image,
                PackSize = product.PackSize,
                UnitSize = product.UnitSize
            };

            try
            {
                await _context.Products.AddAsync(productToAdd);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var productError = new Error
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };

                return ProductResult.ProductFailed(productError);
            }

            var mapped = ProductDto.From(productToAdd);

            return ProductResult.ProductSuccess(mapped);
            
        }

        public async Task<ProductResult> UpdateProductAsync(int id, ProductDto product)
        {
            var productToUpdate = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);

            if (productToUpdate == null)
            {
                var productError = new Error
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find product with id {id}"
                };

                return ProductResult.ProductFailed(productError);
            }

            productToUpdate.Name = product.Name;
            productToUpdate.Active = product.Active;
            productToUpdate.Brand = product.Brand;
            productToUpdate.Category = product.Category;
            productToUpdate.Image = product.Image;
            productToUpdate.PackSize = product.PackSize;
            productToUpdate.UnitSize = product.UnitSize;
            _context.Products.Update(productToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var productError = new Error
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };

                return ProductResult.ProductFailed(productError);
            }

            var mapped = ProductDto.From(productToUpdate);

            return ProductResult.ProductSuccess(mapped);
        }

        public async Task<ProductResult> RemoveProductAsync(int id)
        {
            var productToUpdate = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);

            if (productToUpdate == null)
            {
                var productError = new Error
                {
                    Code = ErrorCodes.EntityFrameworkDuplicateWarning,
                    Description = $"Could not find product with id {id}"
                };

                return ProductResult.ProductFailed(productError);
            }
            
            productToUpdate.Active = false;

            _context.Products.Update(productToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var productError = new Error
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };

                return ProductResult.ProductFailed(productError);
            }

            return ProductResult.ProductSuccess(null);
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _context.Products.ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> Products(Func<Product, bool> query)
        {
            return await _context.Products.Where(p => query(p)).ToListAsync();
        }
    }
}