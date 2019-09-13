using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;

namespace NeverEmptyPantry.Common.Util
{
    public static class ApiHelper
    {
        public static IActionResult ActionFromOperationResult(IOperationResult result)
        {
            if (result.Succeeded)
            {
                return new OkObjectResult(result);
            }

            return new BadRequestObjectResult(result);
        }
    }
}