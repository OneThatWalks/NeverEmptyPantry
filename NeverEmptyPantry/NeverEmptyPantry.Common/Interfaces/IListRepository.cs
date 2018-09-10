using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IListRepository
    {
        Task<IEnumerable<List>> GetListsAsync();
        Task<IEnumerable<List>> GetListsAsync(Func<List, bool> query);
        Task<List> GetListAsync(int id);
        Task<ListResult> AddListAsync(ListDto list);
        Task<ListResult> UpdateListAsync(int id, ListDto list);
        Task<ListResult> DeleteListAsync(int id);
    }
}