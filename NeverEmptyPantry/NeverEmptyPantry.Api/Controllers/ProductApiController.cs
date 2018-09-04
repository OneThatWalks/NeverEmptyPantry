using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Product;
using NeverEmptyPantry.Common.Util;
using System.Linq;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Api.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Produces("application/json")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _product;

        public ProductApiController(IProductService product)
        {
            _product = product;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct(ProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            var result = await _product.AddProduct(model);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, ProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            var result = await _product.UpdateProduct(id, model);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}/remove")]
        public async Task<IActionResult> RemoveProduct([FromRoute] int id)
        {
            var result = await _product.RemoveProduct(id);

            return ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("")]
        public async Task<IActionResult> GetProducts()
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            var result = await _product.GetProducts();

            return !result.Products.Any() ? new NoContentResult() : ApiHelper.ActionFromResult(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var result = await _product.GetProduct(id);

            return result.Product == null ? new NotFoundResult() : ApiHelper.ActionFromResult(result);
        }
    }
}