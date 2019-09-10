using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;
using NeverEmptyPantry.Repository.Entity;

namespace NeverEmptyPantry.Repository.Services
{
    [ExcludeFromCodeCoverage]
    public class ListProductRepository : IListProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ListProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ListProductsResult> GetListProductsAsync(int listId)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {listId}"
                };
                return ListProductsResult.ListProductsFailed(err);
            }

            var products = await _context.ListProducts.Include("Product").Where(p => p.List == list).ToListAsync();

            return ListProductsResult.ListProductsSuccess(products.Select(ListProductDto.From).ToArray());
        }


        public async Task<ListProductResult> GetListProductAsync(int listId, int productId)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            var product = await _context.ListProducts.Include("Product").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == productId);

            if (product == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find product with id {productId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            return ListProductResult.ListProductSuccess(ListProductDto.From(product), null);
        }

        public async Task<ListProductResult> GetListProductAsync(int listProductId)
        {
            throw new NotImplementedException();
        }


        public async Task<ListProductResult> AddListProductAsync(int listId, Product product)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            var existsingListProduct = await _context.ListProducts.Include("Product").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == product.Id);

            if (existsingListProduct != null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkDuplicateWarning,
                    Description = $"Product with id {product.Id} already exists on list with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            if (product == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find product with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            var listProductToAdd = ListProduct.From(product, list);

            try
            {
                await _context.ListProducts.AddAsync(listProductToAdd);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListProductResult.ListProductFailed(listError);
            }

            return ListProductResult.ListProductSuccess(ListProductDto.From(listProductToAdd), null);
        }


        public async Task<ListProductResult> RemoveListProductAsync(int listId, int productId)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            var product = await _context.ListProducts.Include("Product").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == productId);

            if (product == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find product with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            product.AuditDateTime = DateTime.UtcNow;
            product.ListProductState = ListProductState.ITEM_REMOVED;

            _context.ListProducts.Update(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListProductResult.ListProductFailed(listError);
            }

            return ListProductResult.ListProductSuccess(ListProductDto.From(product), null);
        }


        public async Task<ListProductResult> UpdateListProductStateAsync(int listId, int productId, ListProductState state)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            var product = await _context.ListProducts.Include("Product").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == productId);

            if (product == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find product with id {listId}"
                };
                return ListProductResult.ListProductFailed(err);
            }

            product.AuditDateTime = DateTime.UtcNow;
            product.ListProductState = state;

            _context.ListProducts.Update(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListProductResult.ListProductFailed(listError);
            }

            return ListProductResult.ListProductSuccess(ListProductDto.From(product), null);
        }
    }
}