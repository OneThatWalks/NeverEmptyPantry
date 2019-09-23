using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces.Entity;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IEntityService<T> where T : IBaseEntity
    {
        Task<IOperationResult<T>> CreateAsync(T entity);
        Task<IOperationResult<IList<T>>> ReadAsync(Func<T, bool> entityQuery);
        Task<IOperationResult<T>> UpdateAsync(T entity);
        Task<IOperationResult<T>> RemoveAsync(int id);
    }
}