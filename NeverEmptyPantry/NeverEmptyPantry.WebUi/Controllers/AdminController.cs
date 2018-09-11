using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class AdminController : Controller
    {
        private readonly IListService _listService;
        private readonly IListProductService _listProductService;
        private readonly IUserVoteService _userVoteService;

        public AdminController(IListService listService, IListProductService listProductService, IUserVoteService userVoteService)
        {
            _listService = listService;
            _listProductService = listProductService;
            _userVoteService = userVoteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminIndexViewModel();

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
                    var test = new OkObjectResult(new ChartDataSet[]
                {
                    new ChartDataSet
                    {
                        BackgroundColor = Color.AliceBlue,
                        BorderColor = Color.Aqua,
                        Label =  "Ice" ,
                        Data = new ChartPointLocationRadius[] { new ChartPointLocationRadius
                        {
                            X = 10,
                            Y = 5,
                            R = 5
                        } }
                    },
                    new ChartDataSet
                    {
                        BackgroundColor = Color.BurlyWood,
                        BorderColor = Color.Coral,
                        Label =  "Fire" ,
                        Data = new ChartPointLocationRadius[] {new ChartPointLocationRadius
                        {
                            X = 5,
                            Y = 10,
                            R = 7
                        }}
                    },
                    new ChartDataSet
                    {
                        BackgroundColor = Color.MediumOrchid,
                        BorderColor = Color.Indigo,
                        Label =  "Purple" ,
                        Data = new ChartPointLocationRadius[] {new ChartPointLocationRadius
                        {
                            X = 15,
                            Y = 15,
                            R = 10
                        }}
                    }
                });
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