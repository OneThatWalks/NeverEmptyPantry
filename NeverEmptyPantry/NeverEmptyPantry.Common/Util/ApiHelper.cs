using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public static IActionResult ActionFromOperationResult<T>(IOperationResult<T> result)
        {
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result);
            }

            if (result.Data == null || IsEnumerable(typeof(T)) && !((IEnumerable<object>)result.Data).Any())
            {
                return new NotFoundObjectResult(result);
            }

            return new OkObjectResult(result);
        }

        private static bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}