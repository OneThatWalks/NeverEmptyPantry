using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Util;
using System.Linq;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Api.Controllers
{
    [Authorize]
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            // Get all products
            var operationResult = await _productService.ReadAsync(p => true);

            return Ok(operationResult);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            // Get all products
            var operationResult = await _productService.ReadAsync(p => p.Id == id);

            var newOperationResult = new OperationResult<Product>()
            {
                Data = operationResult.Data.FirstOrDefault(),
                Succeeded = operationResult.Succeeded,
                Errors = operationResult.Errors
            };

            return ApiHelper.ActionFromOperationResult(newOperationResult);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(Product product)
        {
            if (!ModelState.IsValid)
            {
                var error = new OperationError
                {
                    Name = "Invalid",
                    Description = "Model is not valid"
                };

                return BadRequest(OperationResult.Failed(error));
            }

            var productUpdateResult = await _productService.UpdateAsync(product);

            return ApiHelper.ActionFromOperationResult(productUpdateResult);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                var error = new OperationError
                {
                    Name = "Invalid",
                    Description = "Model is not valid"
                };

                return BadRequest(OperationResult.Failed(error));
            }

            var productCreateResult = await _productService.CreateAsync(product);

            return ApiHelper.ActionFromOperationResult(productCreateResult);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var productCreateResult = await _productService.RemoveAsync(id);

            return ApiHelper.ActionFromOperationResult(productCreateResult);
        }
    }
}