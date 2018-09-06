using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.WebUi.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminIndexViewModel();

            return
            View("Index", model);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> AdminMetrics(string type)
        {
            if ("itempop".Equals(type))
            {
                return new OkObjectResult(new ChartDataSet[]
                {
                    new ChartDataSet
                    {
                        BackgroundColor = Color.AliceBlue,
                        BorderColor = Color.Aqua,
                        Label = new string[] { "Ice" },
                        Data = new ChartPointLocationRadius
                        {
                            X = 10,
                            Y = 5,
                            R = 5
                        }
                    },
                    new ChartDataSet
                    {
                        BackgroundColor = Color.BurlyWood,
                        BorderColor = Color.Coral,
                        Label = new string[] { "Fire" },
                        Data = new ChartPointLocationRadius
                        {
                            X = 5,
                            Y = 10,
                            R = 7
                        }
                    },
                    new ChartDataSet
                    {
                        BackgroundColor = Color.MediumOrchid,
                        BorderColor = Color.Indigo,
                        Label = new string[] { "Purple" },
                        Data = new ChartPointLocationRadius
                        {
                            X = 15,
                            Y = 15,
                            R = 10
                        }
                    }
                });
            }

            return new BadRequestResult();
        }
    }
}