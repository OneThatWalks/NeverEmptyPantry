using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
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

        public static IActionResult ActionFromOperationResult<T>(IOperationResult<T> result)
        {

            if (result.Succeeded)
            {
                return new OkObjectResult(result);
            } else if (result.Data == null || IsEnumerable(typeof(T)) && !((IEnumerable)result.Data).Any())
            {
                return new NotFoundObjectResult(result);
            }

            return new BadRequestObjectResult(result);
        }

        private static bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}