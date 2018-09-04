using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Util;

namespace NeverEmptyPantry.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Administrator")]
    public class RolesApiController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public RolesApiController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        [HttpGet("{email}/list")]
        public async Task<IActionResult> AccountRoles([FromRoute] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new BadRequestResult();
            }

            var result = await _accountService.GetUserRoles(email);

            return new OkObjectResult(result);
        }
        
        [HttpPost("{email}/add")]
        public async Task<IActionResult> AddRoleToUser([FromRoute] string email, [FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                return new BadRequestResult();
            }

            var result = await _accountService.AddUserToRole(email, name);

            return ApiHelper.ActionFromResult(result);
        }

        [HttpPost("{email}/remove")]
        public async Task<IActionResult> RemoveUserFromRole([FromRoute] string email, [FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                return new BadRequestResult();
            }

            var result = await _accountService.RemoveUserFromRole(email, name);

            return ApiHelper.ActionFromResult(result);
        }

        [HttpGet("list")]
        public IActionResult Roles()
        {
            var roles = _accountService.GetRoles();

            return new OkObjectResult(roles);
        }
        
        [HttpPost("add")]
        public async Task<IActionResult> AddRole([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestResult();
            }

            var roleCreationResult = await _accountService.CreateRole(name);

            if (!roleCreationResult.Succeeded)
            {
                return new BadRequestObjectResult(roleCreationResult);
            }

            return new OkObjectResult(roleCreationResult);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestResult();
            }

            var roleRemoveResult = await _accountService.RemoveRole(name);

            if (!roleRemoveResult.Succeeded)
            {
                return new BadRequestObjectResult(roleRemoveResult);
            }

            return new OkObjectResult(roleRemoveResult);
        }

        [HttpPost("{role}/update")]
        public async Task<IActionResult> UpdateRole([FromRoute] string role, [FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new BadRequestResult();
            }

            var roleRemoveResult = await _accountService.UpdateRole(role, name);

            if (!roleRemoveResult.Succeeded)
            {
                return new BadRequestObjectResult(roleRemoveResult);
            }

            return new OkObjectResult(roleRemoveResult);
        }
    }
}