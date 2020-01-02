using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Admin;
using NeverEmptyPantry.Common.Util;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Api.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Administrator)]
    public class AdminController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;

        public AdminController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        [HttpGet("roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Roles()
        {
            var roles = await _administratorService.GetRolesAsync();

            return ApiHelper.ActionFromOperationResult(roles);
        }

        [HttpGet("permissions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Permissions()
        {
            var permissions = await _administratorService.GetPermissionsAsync();

            return ApiHelper.ActionFromOperationResult(permissions);
        }

        [HttpPut("roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModel viewModel)
        {
            var result = await _administratorService.UpdateRoleAsync(viewModel);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpPost("roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddRole([FromBody] RoleViewModel viewModel)
        {
            var result = await _administratorService.AddRoleAsync(viewModel);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpDelete("roles/{roleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveRole(string roleId)
        {
            var result = await _administratorService.RemoveRoleAsync(roleId);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Users()
        {
            var result = await _administratorService.GetUsersAsync();

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpPut("users/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] ProfileModel model)
        {
            var result = await _administratorService.UpdateUserAsync(userId, model);

            return ApiHelper.ActionFromOperationResult(result);
        }

    }
}