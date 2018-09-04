using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Util;

namespace NeverEmptyPantry.Api.Controllers
{
    [Route("api/list")]
    [ApiController]
    [Produces("application/json")]
    public class ListApiController : ControllerBase
    {
        private readonly IListService _listService;
        private readonly IListProductService _listProductService;

        public ListApiController(IListService listService, IListProductService listProductService)
        {
            _listService = listService;
            _listProductService = listProductService;
        }

        #region List CRUD operations

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<IActionResult> GetLists()
        {
            var lists = await _listService.GetLists();

            if (!lists.Lists.Any())
            {
                return new NoContentResult();
            }

            return new OkObjectResult(lists);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetList([FromRoute] int id)
        {
            var list = await _listService.GetList(id);

            return ApiHelper.ActionFromResult(list);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add")]
        public async Task<IActionResult> AddList([FromBody] ListDto model)
        {
            var result = await _listService.CreateList(model);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateList([FromRoute] int id, [FromBody] ListDto model)
        {
            var result = await _listService.UpdateList(model);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}/remove")]
        public async Task<IActionResult> DeleteList([FromRoute] int id)
        {
            var result = await _listService.RemoveList(id);

            return ApiHelper.ActionFromResult(result);
        }

        #endregion

        #region ListProduct CRUD Operations

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{listId}/product")]
        public async Task<IActionResult> GetListProducts([FromRoute] int listId)
        {
            var listProducts = await _listProductService.GetListProducts(listId);

            return ApiHelper.ActionFromResult(listProducts);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{listId}/product/{productId}")]
        public async Task<IActionResult> GetListProduct([FromRoute] int listId, [FromRoute] int productId)
        {
            var listProduct = await _listProductService.GetListProduct(listId, productId);

            return ApiHelper.ActionFromResult(listProduct);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{listId}/product/{productId}/add")]
        public async Task<IActionResult> AddListProduct([FromRoute] int listId, [FromRoute] int productId)
        {
            var result = await _listProductService.CreateListProduct(listId, 0/*FIXME*/);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{listId}/product/{productId}/remove")]
        public async Task<IActionResult> RemoveListProduct([FromRoute] int listId, [FromRoute] int productId)
        {
            var result = await _listProductService.RemoveListProduct(listId, productId);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{listId}/product/{productId}/update")]
        public async Task<IActionResult> UpdateListProduct([FromRoute] int listId, [FromRoute] int productId, [FromQuery] string state)
        {
            if (!Enum.TryParse(typeof(ListProductState), state, true, out var productState))
            {
                var err = new Error
                {
                    Code = ErrorCodes.EnumParseError,
                    Description = "Could not parse argument {state}."
                };
                return new BadRequestObjectResult(ListProductResult.ListProductFailed(err));
            }

            var result = await _listProductService.UpdateListProduct(listId, productId, (ListProductState)productState);

            return ApiHelper.ActionFromResult(result);
        }

        #endregion


    }
}