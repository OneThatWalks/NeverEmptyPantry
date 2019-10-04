using NeverEmptyPantry.Common.Interfaces.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IEntityService<T> where T : IBaseEntity
    {
        Task<IOperationResult<T>> CreateAsync(T entity);
        Task<IOperationResult<IList<T>>> ReadAsync(Expression<Func<T, bool>> query);
        Task<IOperationResult<T>> UpdateAsync(T entity);
        Task<IOperationResult<T>> RemoveAsync(int id);
    }
}