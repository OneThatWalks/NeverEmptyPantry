using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Util;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Api.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
        {
            var loginResult = await _authenticationService.AuthenticateAsync(model);

            return loginResult.Succeeded ? (IActionResult)Ok(loginResult) : Unauthorized();
        }

        [AllowAnonymous]
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
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Profile()
        {
            var profile = await _accountService.GetProfileAsync(User.FindFirst(JwtRegisteredClaimNames.UniqueName).Value);

            return ApiHelper.ActionFromOperationResult(profile);
        }

        [HttpPut("profile")]
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