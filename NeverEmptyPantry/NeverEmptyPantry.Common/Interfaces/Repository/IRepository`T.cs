using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces.Entity;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IRepository<T> where T : IBaseEntity
    {
        /// <summary>
        /// Creates the entity
        /// </summary>
        /// <param name="entity">The entity to create</param>
        /// <param name="userId">The user id associated with the change</param>
        /// <returns>A task result that represents the newly created entity</returns>
        Task<T> CreateAsync(T entity, string userId);

        /// <summary>
        /// Reads an entity
        /// </summary>
        /// <param name="entityId">The id of the entity to read</param>
        /// <returns>A task result that represents the completion of the read operation</returns>
        Task<T> ReadAsync(int entityId);

        /// <summary>
        /// Reads entities
        /// </summary>
        /// <param name="query">The query to filter entities by</param>
        /// <returns>A task result that represents filtered entities</returns>
        Task<List<T>> ReadAsync(Expression<Func<T, bool>> query);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to be updated</param>
        /// <param name="userId">The user id associated with the change</param>
        /// <returns>A task result that represents the updated entity</returns>
        Task<T> UpdateAsync(T entity, string userId);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <param name="userId">The user id associated with the change</param>
        /// <returns>A task result that represents the deleted entity</returns>
        Task<T> RemoveAsync(T entity, string userId);
    }
}