using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.Common.Util
{
    public static class ApiHelper
    {
        public static IActionResult ActionFromResult(Result result)
        {
            if (result.Succeeded)
            {
                return new OkObjectResult(result);
            }
            return new BadRequestObjectResult(result);
        }
    }
}