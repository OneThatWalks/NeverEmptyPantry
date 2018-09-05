using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class AdminController : Controller
    {
        // GET
        public IActionResult Index()
        {
            var model = new AdminIndexViewModel();

            return
            View("Index", model);
        }
    }
}