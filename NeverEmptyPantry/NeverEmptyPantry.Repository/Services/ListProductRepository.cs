using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        public async Task<IEnumerable<ListProductMap>> GetListProductsAsync(int listId)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                throw new Exception($"List not found with id {listId}");
            }

            var products = await _context.ListProducts.Include("Product").Include("List").Where(p => p.List == list).ToListAsync();

            return products;
        }

        public async Task<ListProductMap> GetListProductAsync(int listId, int productId)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listId);

            if (list == null)
            {
                throw new Exception($"List not found with id {listId}");
            }

            var product = await _context.ListProducts.Include("Product").Include("List").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == productId);

            if (product == null)
            {
                throw new Exception($"Product not found with id {productId}");
            }

            return product;
        }

        public async Task<ListProductMap> AddListProductAsync(ListProductMap listProductMap)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listProductMap.List.Id);

            if (list == null)
            {
                throw new Exception($"List not found with id {listProductMap.List.Id}");
            }

            var existingListProduct = await _context.ListProducts.Include("Product").Include("List").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == listProductMap.Product.Id);

            if (existingListProduct != null)
            {
                // Already exists
                return existingListProduct;
            }

            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == listProductMap.Product.Id);

            if (product == null)
            {
                throw new Exception($"Product not found with id {listProductMap.Product.Id}");
            }

            EntityEntry<ListProductMap> entityResult;

            try
            {
                entityResult = await _context.ListProducts.AddAsync(listProductMap);
                await _context.AuditLog.AddAsync(AuditLog.From(listProductMap, AuditAction.CREATE, null));

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityResult.Entity;
        }


        public async Task<ListProductMap> RemoveListProductAsync(ListProductMap listProductMap, bool hardDelete = false)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listProductMap.List.Id);

            if (list == null)
            {
                throw new Exception($"List not found with id {listProductMap.List.Id}");
            }

            var listProduct = await _context.ListProducts.Include("Product").Include("List").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == listProductMap.Product.Id);

            if (listProduct == null)
            {
                throw new Exception($"List product not found with id {listProductMap.Product.Id}");
            }

            EntityEntry<ListProductMap> entityResult;

            try
            {
                if (hardDelete)
                {
                    await _context.AuditLog.AddAsync(AuditLog.From(listProduct, AuditAction.HARD_DELETE, null));
                    entityResult = _context.ListProducts.Remove(listProduct);
                }
                else
                {
                    await _context.AuditLog.AddAsync(AuditLog.From(listProduct, AuditAction.SOFT_DELETE, null));
                    entityResult = _context.ListProducts.Update(listProduct);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityResult.Entity;
        }


        public async Task<ListProductMap> UpdateListProductMap(ListProductMap listProductMap)
        {
            var list = await _context.Lists.SingleOrDefaultAsync(l => l.Id == listProductMap.List.Id);

            if (list == null)
            {
                throw new Exception($"List not found with id {listProductMap.List.Id}");
            }

            var listProduct = await _context.ListProducts.Include("Product").Include("List").Where(p => p.List == list).SingleOrDefaultAsync(y => y.Product.Id == listProductMap.Product.Id);

            if (listProduct == null)
            {
                throw new Exception($"List product not found with id {listProductMap.Product.Id}");
            }

            EntityEntry<ListProductMap> entityResult;

            try
            {
                entityResult = _context.ListProducts.Update(listProduct);
                await _context.AuditLog.AddAsync(AuditLog.From(listProductMap, AuditAction.UPDATE, null));

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityResult.Entity;
        }
    }
}