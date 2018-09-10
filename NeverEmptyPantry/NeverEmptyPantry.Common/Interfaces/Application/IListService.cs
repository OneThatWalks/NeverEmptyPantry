using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IListService
    {
        Task<ListsResult> GetLists(Func<List, bool> query);
        Task<ListsResult> GetLists();
        Task<ListResult> GetList(int listId);
        Task<ListResult> CreateList(ListDto model);
        Task<ListResult> RemoveList(int listId);
        Task<ListResult> UpdateList(ListDto model);

        Task<ListResult> ProcessList(OrderProcessingViewModel model);
    }
}