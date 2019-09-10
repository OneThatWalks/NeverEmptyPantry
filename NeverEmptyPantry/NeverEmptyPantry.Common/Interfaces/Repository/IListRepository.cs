using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IListRepository
    {
        /// <summary>
        /// Gets all of the lists
        /// </summary>
        /// <returns>A task result that represents a list of lists</returns>
        Task<IEnumerable<List>> GetListsAsync();

        /// <summary>
        /// Gets lists based on a query function
        /// </summary>
        /// <param name="query">The query to filter lists by</param>
        /// <returns>A task result that represents the filtered lists</returns>
        Task<IEnumerable<List>> GetListsAsync(Func<List, bool> query);

        /// <summary>
        /// Gets a list by a list id
        /// </summary>
        /// <param name="id">The id of the list to query</param>
        /// <returns>A task result that represents the list</returns>
        Task<List> GetListAsync(int id);

        /// <summary>
        /// Adds a list
        /// </summary>
        /// <param name="list">The list to add</param>
        /// <returns>A task result that represents the added list</returns>
        Task<List> AddListAsync(List list);

        /// <summary>
        /// Updates a list
        /// </summary>
        /// <param name="list">The list to update</param>
        /// <returns>A task result that represents the updated list</returns>
        Task<List> UpdateListAsync(List list);

        /// <summary>
        /// Removes a list
        /// </summary>
        /// <param name="list">The list to remove</param>
        /// <param name="hardDelete">Whether to remove the record or mark inactive</param>
        /// <returns>A task result that represents the removed list</returns>
        Task<List> DeleteListAsync(List list, bool hardDelete = false);
    }
}