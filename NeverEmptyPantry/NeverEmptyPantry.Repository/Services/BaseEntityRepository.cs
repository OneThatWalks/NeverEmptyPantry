using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Repository.Services
{
    public class BaseEntityRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepository _accountRepository;

        public BaseEntityRepository(ApplicationDbContext context, IAccountRepository accountRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
        }

        public async Task<T> CreateAsync(T entity, string userId)
        {
            EntityEntry<T> entityResult;

            try
            {
                // Mark the entity as an add and track
                entityResult = await _context.Set<T>().AddAsync(entity);

                // Create and add the audit entry
                var user = await _accountRepository.GetUserOrSystemAsync(userId);

                await _context.AuditLog.AddAsync(AuditLog.From(entityResult, AuditAction.CREATE, user));

                // Save context changes
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityResult.Entity;
        }

        public Task<T> ReadAsync(int entityId)
        {
            return _context.Set<T>().SingleOrDefaultAsync(p => p.Id == entityId);
        }

        public Task<List<T>> ReadAsync(Expression<Func<T, bool>> query)
        {
            return _context.Set<T>().Where(query).ToListAsync();
        }

        public async Task<T> UpdateAsync(T entity, string userId)
        {
            var contextEntity = await _context.Set<T>().SingleOrDefaultAsync(p => p.Id == entity.Id);

            if (contextEntity == null)
            {
                throw new Exception($"Entity not found with id {entity.Id}");
            }

            EntityEntry<T> entityEntry;

            try
            {
                // Merge the mergeable properties into the context entity
                contextEntity.MergeProperties(entity);

                // Update the context
                entityEntry = _context.Set<T>().Update(contextEntity);

                // Create and add the audit entry
                var user = await _accountRepository.GetUserOrSystemAsync(userId);

                await _context.AuditLog.AddAsync(AuditLog.From(entityEntry, AuditAction.UPDATE, user));

                // Save context changes
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityEntry.Entity;
        }

        public async Task<T> RemoveAsync(T entity, string userId)
        {
            var contextEntity = await _context.Set<T>().SingleOrDefaultAsync(p => p.Id == entity.Id);

            if (contextEntity == null)
            {
                throw new Exception($"Entity not found with id {entity.Id}");
            }

            EntityEntry<T> entityEntry;

            try
            {
                // Mark for delete
                entityEntry = _context.Set<T>().Remove(contextEntity);

                // Create and add the audit entry
                var user = await _accountRepository.GetUserOrSystemAsync(userId);

                await _context.AuditLog.AddAsync(AuditLog.From(entityEntry, AuditAction.DELETE, user));

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception("There was an issue saving the entity.  See inner exception for more details.", e);
            }

            return entityEntry.Entity;
        }
    }
}