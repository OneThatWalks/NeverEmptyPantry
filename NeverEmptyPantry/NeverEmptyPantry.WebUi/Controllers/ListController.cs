using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Util;
using NeverEmptyPantry.WebUi.Models;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class ListController : Controller
    {
        private readonly IListService _listService;
        private readonly IProductService _productService;
        private readonly IUserVoteService _voteService;
        private readonly IAccountService _accountService;
        private readonly IListProductService _listProductService;

        public ListController(IListService listService, IProductService productService, IUserVoteService voteService, IAccountService accountService, IListProductService listProductService)
        {
            _listService = listService;
            _productService = productService;
            _voteService = voteService;
            _accountService = accountService;
            _listProductService = listProductService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] OrderState state = OrderState.LIST_CREATED)
        {
            var lists = await _listService.GetLists(x => x.OrderState == state);

            var mappedToViewModels = lists.Lists.Select(Mapper.Map<ListViewModel>);

            return View(mappedToViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ListViewModel
            {
                StartDateTime = DateTime.Now
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var mappedList = Mapper.Map<ListDto>(model);

            var result = await _listService.CreateList(mappedList);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Errors = result.Errors;
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var listResult = await _listService.GetList(id);

            if (!listResult.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = listResult.Errors;
                return RedirectToAction("Index");
            }

            var mappedList = Mapper.Map<ListViewModel>(listResult.List);

            return View(mappedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var mappedList = Mapper.Map<ListDto>(model);

            var result = await _listService.UpdateList(mappedList);

            if (!result.Succeeded)
            {
                // Add errors for the index if an error was encountered
                ViewData.Clear();
                ViewBag.Errors = result.Errors;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var listResult = await _listService.GetList(id);

            if (!listResult.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = listResult.Errors;
                return RedirectToAction("Index");
            }

            var mappedList = Mapper.Map<ListViewModel>(listResult.List);

            if (listResult.ListProducts != null && listResult.ListProducts.Any())
            {
                mappedList.ListProducts = listResult.ListProducts;
            }

            if (listResult.UserProductVotes != null && listResult.UserProductVotes.Any())
            {
                mappedList.UserProductVotes = listResult.UserProductVotes;
            }

            return View(mappedList);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var listResult = await _listService.RemoveList(id);

            if (!listResult.Succeeded)
            {
                // Add errors for the index if an error was encountered
                ViewData.Clear();
                ViewBag.Errors = listResult.Errors;
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Vote(int productId)
        {
            ViewBag.ProductId = productId;

            var listResult = await _listService.GetLists(); // Potential predicate action here

            var lists = listResult.Lists.Where(l => l.OrderState == OrderState.LIST_CREATED).Select(Mapper.Map<ListViewModel>).ToList();

            return View(lists);
        }

        [HttpGet]
        public async Task<IActionResult> CastVote(int id, int product)
        {
            var user = await _accountService.GetUserFromClaimsPrincipal(User);

            var listProductResult = await _listProductService.GetOrCreateListProduct(id, product);

            if (!listProductResult.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = listProductResult.Errors;
                return RedirectToAction("Vote", new { productId = product });
            }

            var result = await _voteService.CreateVote(listProductResult.ListProduct.Id, user, UserProductVoteState.VOTE_NONESSENTIAL);
            
            if (result.Succeeded) {
                return RedirectToAction("Details", "List", new { id = id });
            }

            ViewData.Clear();
            ViewBag.Errors = result.Errors;

            return RedirectToAction("Vote", new { productId = product });
        }

        [HttpGet]
        public async Task<IActionResult> VotePartial(int id)
        {
            var vote = await _voteService.GetVote(id);

            return PartialView("_VoteSeverityModal", vote);
        }

        [HttpPost]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> VoteMultiple(ProductIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            foreach (var productSelect in model.ProductSelects)
            {
                if (!productSelect.IsSelected) continue;

                var lp = await _listProductService.GetOrCreateListProduct(model.ListId, productSelect.ProductId);
                var applicationUser = await _accountService.GetUserFromClaimsPrincipal(User);
                if (lp.Succeeded && applicationUser != null)
                {
                    await _voteService.CreateVote(lp.ListProduct.Id, applicationUser, UserProductVoteState.VOTE_NONESSENTIAL);
                } // TODO: when bad happens
            }

            return new OkResult();
        }

        [HttpGet]
        public async Task<IActionResult> ListProductsPartial(int id)
        {
            var listResult = await _listService.GetList(id);

            if (!listResult.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = listResult.Errors;
                return PartialView("_ListProductsList", null);
            }

            var mappedList = Mapper.Map<ListViewModel>(listResult.List);

            if (listResult.ListProducts != null && listResult.ListProducts.Any())
            {
                mappedList.ListProducts = listResult.ListProducts;
            }

            if (listResult.UserProductVotes != null && listResult.UserProductVotes.Any())
            {
                mappedList.UserProductVotes = listResult.UserProductVotes;
            }

            return PartialView("_ListProductsList", mappedList);
        }
        
        [HttpGet]
        public async Task<IActionResult> RemoveListProduct(int id, int listId)
        {
            await _listProductService.RemoveListProduct(listId, id);

            return RedirectToAction("Details", "List", new {id = listId});
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductSeverity(int id, UserProductVoteState state)
        {
            var result = await _voteService.UpdateVote(id, state);

            return ApiHelper.ActionFromResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveVote(int id, int listId)
        {
            var user = await _accountService.GetUserFromClaimsPrincipal(User);

            var result = await _voteService.RemoveVote(id, user);

            if (!result.Succeeded)
            {
                ViewData.Clear();
                ViewBag.Errors = result.Errors;
            }

            return RedirectToAction("Details", "List", new { id = listId });
        }
    }
}