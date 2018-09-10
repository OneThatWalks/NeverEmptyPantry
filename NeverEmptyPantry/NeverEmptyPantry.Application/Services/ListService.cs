using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Application.Services
{
    public class ListService : IListService
    {
        private readonly IListRepository _listRepository;
        private readonly IListProductRepository _listProductRepository;
        private readonly IUserVoteRepository _userVoteRepository;

        public ListService(IListRepository listRepository, IListProductRepository listProductRepository, IUserVoteRepository userVoteRepository)
        {
            _listRepository = listRepository;
            _listProductRepository = listProductRepository;
            _userVoteRepository = userVoteRepository;
        }

        public async Task<ListsResult> GetLists(Func<List, bool> query)
        {
            var lists = await _listRepository.GetListsAsync(query);

            var mapped = lists.Select(ListDto.From);

            return ListsResult.ListsSuccess(mapped);
        }

        public async Task<ListsResult> GetLists()
        {
            var lists = await _listRepository.GetListsAsync();

            var mapped = lists.Select(ListDto.From);

            return ListsResult.ListsSuccess(mapped);
        }

        public async Task<ListResult> GetList(int listId)
        {
            var list = await _listRepository.GetListAsync(listId);
            var products = await _listProductRepository.GetListProductsAsync(list.Id);
            var votes = await _userVoteRepository.GetListProductVotesAsync(list.Id);

            var mapped = ListDto.From(list);
            var mappedProducts = products.ListProducts.ToList();
            var mappedVotes = votes;

            return ListResult.ListSuccess(mapped, mappedProducts, mappedVotes);
        }

        public async Task<ListResult> CreateList(ListDto model)
        {
            if (model.OrderState != OrderState.LIST_CREATED)
            {
                // When creating we must always set this
                model.OrderState = OrderState.LIST_CREATED;
            }

            var list = await _listRepository.AddListAsync(model);

            return list;
        }

        public async Task<ListResult> RemoveList(int listId)
        {
            var removedList = await _listRepository.DeleteListAsync(listId);

            return removedList;
        }

        public async Task<ListResult> UpdateList(ListDto model)
        {
            var list = await _listRepository.UpdateListAsync(model.Id, model);

            return list;
        }

        public async Task<ListResult> ProcessList(OrderProcessingViewModel model)
        {

            var list = await _listRepository.GetListAsync(model.ListId);
            list.OrderState = OrderState.LIST_PROCESSED;

            foreach (var productVoteGroup in model.ProductVoteGroups)
            {
                await _listProductRepository.UpdateListProductStateAsync(list.Id,
                    productVoteGroup.ProductId, productVoteGroup.IsSelected ? ListProductState.ITEM_ORDERED : ListProductState.ITEM_REJECTED);
            }

            var result = await _listRepository.UpdateListAsync(list.Id, ListDto.From(list));

            return result;
        }
    }
}