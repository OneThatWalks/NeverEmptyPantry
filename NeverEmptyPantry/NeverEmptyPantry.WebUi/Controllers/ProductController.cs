using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Product;
using NeverEmptyPantry.WebUi.Models;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _product;
        private readonly IUserVoteService _voteService;
        private readonly IListProductService _listProductService;
        private readonly IMapper _mapper;

        public ProductController(IProductService product, IUserVoteService voteService, IListProductService listProductService, IMapper mapper)
        {
            _product = product;
            _voteService = voteService;
            _listProductService = listProductService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var productsResult = await _product.GetProducts();

            var mappedToProductVieModels = productsResult.Products.Select(_mapper.Map<ProductViewModel>);

            return View(mappedToProductVieModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var items = Enum.GetValues(typeof(Category)).Cast<Category>()
                .Select(c => new SelectListItem { Text = c.ToString(), Value = ((int)c).ToString() }).ToList();
            var select =  new SelectList(items, "Value", "Text");

            var model = new ProductViewModel {Categories = select};

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            // Do this for when we have errors and need to reset this select
            var items = Enum.GetValues(typeof(Category)).Cast<Category>()
                .Select(c => new SelectListItem { Text = c.ToString(), Value = ((int)c).ToString() }).ToList();
            var select = new SelectList(items, "Value", "Text");
            model.Categories = select;

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            var mappedProduct = _mapper.Map<ProductDto>(model);

            var createResult = await _product.AddProduct(mappedProduct);

            if (createResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ViewData.Clear();
            ViewBag.Error = "There was an issue creating the product";
            
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Remove([FromQuery] int productId)
        {
            var result = await _product.RemoveProduct(productId);

            return result.Succeeded ? RedirectToAction("Index") : RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromQuery] int productId)
        {
            var result = await _product.GetProduct(productId);

            if (result.Succeeded)
            {
                return View(_mapper.Map<ProductViewModel>(result.Product));
            }

            ViewData.Clear();
            ViewBag.Error = result.Errors.FirstOrDefault()?.Description;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromQuery] int productId)
        {
            var result = await _product.GetProduct(productId);

            if (result.Succeeded)
            {
                return View(_mapper.Map<ProductViewModel>(result.Product));
            }

            ViewData.Clear();
            ViewBag.Error = result.Errors.FirstOrDefault()?.Description;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var mapped = _mapper.Map<ProductDto>(model);

            var result = await _product.UpdateProduct(model.Id, mapped);

            if (result.Succeeded)
            {
                return RedirectToAction("Details", new {productId = result.Product.Id});
            }

            ViewData.Clear();
            ViewBag.Error = result.Errors.FirstOrDefault()?.Description;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Vote(int id)
        {
            return RedirectToAction("Vote", "List", new {productId = id});
        }

        [HttpGet]
        public async Task<IActionResult> IndexPartial(int listId)
        {
            var products = await _product.GetProducts();

            var listProducts = await _listProductService.GetListProducts(listId);

            var filtered = products.Products.Where(product => product.Active && listProducts.ListProducts.All(lp => lp.Product.Id != product.Id));

            var toNewModel = filtered.Select(item => new ProductSelect
            {
                IsSelected = false,
                ProductId = item.Id,
                ProductName = item.Name
            }).ToList();

            var view = new ProductIndexViewModel
            {
                ProductSelects = toNewModel,
                ListId = listId
            };

            return PartialView("_ProductIndexPartial", view);
        }
    }
}