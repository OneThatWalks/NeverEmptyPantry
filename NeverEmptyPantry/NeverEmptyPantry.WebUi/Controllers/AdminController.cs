using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class AdminController : Controller
    {
        private readonly IListService _listService;
        private readonly IListProductService _listProductService;
        private readonly IUserVoteService _userVoteService;
        private readonly IAccountService _accountService;

        public AdminController(IListService listService, IListProductService listProductService, IUserVoteService userVoteService, IAccountService accountService)
        {
            _listService = listService;
            _listProductService = listProductService;
            _userVoteService = userVoteService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminIndexViewModel();

            var lists = await _listService.GetLists();

            model.ActiveLists = $"{lists.Lists.Count(list => list.OrderState == OrderState.LIST_CREATED)}/{lists.Lists.Count()}";
            model.ProcessedLists =
                $"{lists.Lists.Count(list => list.OrderState == OrderState.LIST_PROCESSED || list.OrderState == OrderState.LIST_PENDING)}/{lists.Lists.Count()}";
            var recentLists = lists.Lists.Where(list => list.AuditDateTime >= DateTime.UtcNow.AddDays(-7) && list.OrderState == OrderState.LIST_RECEIVED).ToList();
            var recentItems = 0;
            foreach (var recentList in recentLists)
            {
                var items = await _listProductService.GetListProducts(recentList.Id);
                recentItems = recentItems + items.ListProducts.Count();
            }
            model.DeliveredItems = $"{recentItems}";

            var allVotes = await _userVoteService.Votes(item => true); // Whoa this could be bad

            var contributors = allVotes.UserProductVotes.GroupBy(vote => vote.ApplicationUser.Email).OrderByDescending(group => group.Count()).Take(5).ToArray();
            model.Contributors = new List<ProfileDto>();
            foreach (var grouping in contributors)
            {
                var profile = await _accountService.GetProfileAsync(grouping.Key);
                model.Contributors.Add(profile.Profile);
            }


            return
            View("Index", model);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> AdminMetrics(string type)
        {
            var lists = await _listService.GetLists();

            var listProducts = new List<ListProductDto>();

            foreach (var list in lists.Lists)
            {
                var listProductsFromList = await _listProductService.GetListProducts(list.Id);

                listProducts.AddRange(listProductsFromList.ListProducts);
            }

            var listProductsGroupedByProduct = listProducts.GroupBy(x => x.Product.Name);
            var listCount = lists.Lists.Count();
            var dataSets = new List<ChartDataSet>();

            foreach (var grouping in listProductsGroupedByProduct)
            {

                var color = Color.Black;
                var highlight = Color.Gray;

                var r = await GetVoteCount(grouping);
                var x = await GetAverageSeverity(grouping);
                var y = await GetListOccurence(grouping, listCount);

                var chartPoint = new ChartPointLocationRadius
                {
                    R = r,
                    X = x,
                    Y = y
                };

                var chartItem = new ChartDataSet
                {
                    BackgroundColor = color,
                    BorderColor = highlight,
                    Label = $"{grouping.Key}",
                    Data = new ChartPointLocationRadius[]
                    {
                        chartPoint
                    }
                };

                dataSets.Add(chartItem);
            }

            if ("itempop".Equals(type))
            {
                return new OkObjectResult(dataSets.ToArray());
            }

            return new BadRequestResult();
        }

        private async Task<double> GetVoteCount(IGrouping<string, ListProductDto> grouping)
        {
            var d = 0d;

            var ld = new List<double>();

            foreach (var listProductDto in grouping)
            {
                var votes = await _userVoteService.Votes(vote => vote.ListProduct.Id == listProductDto.Id);

                if (!votes.Succeeded) continue;

                ld.Add(votes.UserProductVotes.Length);
            }

            d = ld.Sum();

            return d;
        }

        private async Task<double> GetAverageSeverity(IGrouping<string, ListProductDto> grouping)
        {
            var d = 0d;

            var ld = new List<double>();

            foreach (var listProductDto in grouping)
            {
                var votes = await _userVoteService.Votes(vote => vote.ListProduct.Id == listProductDto.Id);

                if (!votes.Succeeded) continue;

                ld.AddRange(votes.UserProductVotes.Select(vote => (double)vote.UserProductVoteState));
            }

            d = ld.Average();

            return d;
        }

        private async Task<double> GetListOccurence(IGrouping<string, ListProductDto> grouping, int listCount)
        {
            var d = 0d;

            d = (double)grouping.Count() / (double)listCount;
            return d * 100;
        }
    }
}