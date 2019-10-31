using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NeverEmptyPantry.Authorization.Permissions;
using NeverEmptyPantry.Common.Util;

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

        [HttpPost("role/{roleId}/permissions/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddPermissionsToRole(string roleId, [FromBody] IEnumerable<string> model)
        {
            var result = await _administratorService.AddPermissionsToRoleAsync(roleId, model);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpDelete("role/{roleId}/permissions/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemovePermissionsFromRole(string roleId, [FromBody] IEnumerable<string> model)
        {
            var result = await _administratorService.RemovePermissionsFromRoleAsync(roleId, model);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpPost("roles/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddRole([FromBody] string model)
        {
            var result = await _administratorService.AddRoleAsync(model);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpPost("roles/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveRole([FromBody] string model)
        {
            var result = await _administratorService.RemoveRoleAsync(model);

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

        [HttpPost("users/{userId}/roles/add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddUserToRole(string userId, [FromBody] string model)
        {
            var result = await _administratorService.AddUserToRoleAsync(userId, model);

            return ApiHelper.ActionFromOperationResult(result);
        }

        [HttpDelete("users/{userId}/roles/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveUserFromRole(string userId, [FromBody] string model)
        {
            var result = await _administratorService.RemoveUserFromRoleAsync(userId, model);

            return ApiHelper.ActionFromOperationResult(result);
        }

    }
}