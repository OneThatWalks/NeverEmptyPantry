using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Util;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Produces("application/json")]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountApiController(
            IAccountService accountService
            )
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var loginResult = await _accountService.LoginAsync(model);

            return ApiHelper.ActionFromResult(loginResult);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var error = new OperationError
                {
                    Code = "900",
                    Description = "Registration model is not valid"
                };

                return ApiHelper.ActionFromResult(ProfileResult.ProfileFailed(error));
            }

            var registrationResult = await _accountService.RegisterAsync(model);

            return ApiHelper.ActionFromResult(registrationResult);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile([FromQuery] string email)
        {
            var claimsPrincipal = HttpContext.User;

            if (!string.IsNullOrEmpty(email))
            {
                if (!claimsPrincipal.IsInRole("Administrator"))
                {
                    var error = new OperationError
                    {
                        Code = "900",
                        Description = "User does not have high enough priveledges"
                    };

                    return ApiHelper.ActionFromResult(ProfileResult.ProfileFailed(error));
                }

                var profileResult = await _accountService.GetProfileAsync(email);

                return ApiHelper.ActionFromResult(profileResult);
            }

            var managerUser = await _accountService.GetUserFromClaimsPrincipal(HttpContext.User);

            var profile = ProfileDto.From(managerUser);

            return ApiHelper.ActionFromResult(ProfileResult.ProfileSuccess(profile));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("profile")]
        public async Task<IActionResult> Profile([FromBody] ProfileDto model) // TODO: admin can update profiles
        {
            if (!ModelState.IsValid)
            {
                var error = new OperationError
                {
                    Code = "900",
                    Description = "Registration model is not valid"
                };

                return ApiHelper.ActionFromResult(ProfileResult.ProfileFailed(error));
            }

            var profileResult = await _accountService.SetProfileAsync(model); // TODO check security of serialization for dto

            return ApiHelper.ActionFromResult(profileResult);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResult = await _accountService.LogoutAsync();

            return ApiHelper.ActionFromResult(logoutResult);
        }
    }
}