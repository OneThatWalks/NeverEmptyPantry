using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models.Account;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.WebUi.Models;
using System.Linq;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces.Application;

namespace NeverEmptyPantry.WebUi.Controllers
{

    [Route("account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllUsers();

            return View(users);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile([FromQuery] string email)
        {
            var currentUserClaim = HttpContext.User;

            if (!string.IsNullOrEmpty(email))
            {
                if (!currentUserClaim.IsInRole("Administrator"))
                {
                    return new BadRequestResult();
                }

                var profile = await _accountService.GetProfileAsync(email);

                if (!profile.Succeeded)
                {
                    return new BadRequestResult();
                }

                profile.Profile.Roles = await _accountService.GetUserRoles(email);

                var view = Mapper.Map<ProfileViewModel>(profile.Profile);

                var roles = await _accountService.GetRoles();

                view.AllRoles = roles.Select(i => new string(i.Name));

                return View(view);
            }
            
            var managerUser = await _accountService.GetUserFromClaimsPrincipal(currentUserClaim);

            var selfView = Mapper.Map<ProfileViewModel>(managerUser);

            return View(selfView);
        }

        // TODO: ought to be a post for profile

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = Mapper.Map<LoginDto>(model);

            var result = await _accountService.LoginAsync(dto);

            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;

            var locations = await _accountService.GetOfficeLocations();

            var model = new RegisterViewModel()
            {
                OfficeLocations = locations.ToList()
            };

            return View(model);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = Mapper.Map<RegisterDto>(model);

            var result = await _accountService.RegisterAsync(dto);
            if (result.Succeeded)
            {
                await _accountService.LoginAsync(new LoginDto
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = false
                });

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Errors.First().Description);

            return View(model);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}