using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Util;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NeverEmptyPantry.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAccountService accountService, IAuthenticationService authenticationService)
        {
            _accountService = accountService;
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            var loginResult = await _authenticationService.AuthenticateAsync(model);

            return loginResult.Succeeded ? (IActionResult)Ok(loginResult) : Unauthorized();
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
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

            var registrationResult = await _accountService.RegisterAsync(model);

            return ApiHelper.ActionFromOperationResult(registrationResult);
        }

        //TODO: CLAIM POLICY
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Profile()
        {
            var profile = await _accountService.GetProfileAsync(User.FindFirst(JwtRegisteredClaimNames.UniqueName).Value);

            return ApiHelper.ActionFromOperationResult(profile);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Profile([FromBody] ProfileModel model)
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

            var profileResult = await _accountService.UpdateProfileAsync(User.FindFirst(JwtRegisteredClaimNames.UniqueName).Value, model);

            return ApiHelper.ActionFromOperationResult(profileResult);
        }
    }
}