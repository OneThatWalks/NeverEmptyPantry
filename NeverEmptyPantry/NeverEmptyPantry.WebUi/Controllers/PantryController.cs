using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.WebUi.Models;
using Newtonsoft.Json;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class PantryController : Controller
    {
        private readonly IListService _listService;
        private readonly IListProductService _listProductService;
        private readonly IMapper _mapper;

        public PantryController(IListService listService, IListProductService listProductService, IMapper mapper)
        {
            _listService = listService;
            _listProductService = listProductService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {

            var lists = await _listService.GetLists();

            if (!lists.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = lists.Errors;
                return View();
            }

            var listsMapped = lists.Lists.Select(_mapper.Map<ListViewModel>).ToList();

            var pvm = new PantryViewModel
            {
                ListViewModels = listsMapped
            };

            var activeLists = listsMapped.Where(l => l.OrderState == OrderState.LIST_CREATED).ToList();

            if (activeLists.Any())
            {
                var active = new List<ListViewModel>();

                foreach (var activeList in activeLists)
                {
                    var result = await _listService.GetList(activeList.Id);

                    if (result.Succeeded)
                    {
                        var item = _mapper.Map<ListViewModel>(result.List);

                        item.ListProducts = result.ListProducts;
                        item.UserProductVotes = result.UserProductVotes;

                        active.Add(item);
                    }
                }

                pvm.ActiveLists = active;
            }

            return View(pvm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderProcessing(int id)
        {
            var list = await _listService.GetList(id);

            if (list.Succeeded)
            {
                var votesGroupedByProduct = list.UserProductVotes.GroupBy(vote => vote.ListProduct.Id);
                var groupingRefined = votesGroupedByProduct.Select(group => new ProductVoteGroup
                {
                    ProductId = group.Key,
                    ProductName = list.ListProducts.SingleOrDefault(x => x.Id == group.Key)?.Product.Name,
                    VoteCount = group.Count(),
                    VotingAverage = group.Average(vote => (int)vote.UserProductVoteState)
                });

                var groupingRefinedOrdered = groupingRefined.OrderByDescending(group => group.VoteCount)
                    .ThenByDescending(group => group.VotingAverage).ToList();
                

                var viewModel = new OrderProcessingViewModel
                {
                    ProductVoteGroups = groupingRefinedOrdered,
                    ListId = id
                };

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> OrderProcessing(OrderProcessingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _listService.ProcessList(model);

            return View("Index");
        }
    }
}