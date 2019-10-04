using Microsoft.AspNetCore.Mvc;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Util;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;

namespace NeverEmptyPantry.Tests.Api
{
    [TestFixture]
    public class ApiHelperTests
    {

        [Test]
        public void ActionFromOperationResult_ReturnBadRequest_WhenSuccessFalse()
        {
            // Arrange
            var operationResult = OperationResult<object>.Failed();

            // Act
            var result = ApiHelper.ActionFromOperationResult(operationResult);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public void ActionFromOperationResult_ReturnNotFound_WhenSuccessButNullData()
        {
            // Arrange
            var operationResult = OperationResult<object>.Success(null);

            // Act
            var result = ApiHelper.ActionFromOperationResult(operationResult);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActionFromOperationResult_ReturnNotFound_WhenSuccessButEmptyData()
        {
            // Arrange
            var operationResult = OperationResult<IEnumerable<object>>.Success(new List<object>());

            // Act
            var result = ApiHelper.ActionFromOperationResult(operationResult);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void ActionFromOperationResult_ReturnOk_WhenSuccess()
        {
            // Arrange
            var operationResult = OperationResult<object>.Success(new
            {
                Property = "Property"
            });

            // Act
            var result = ApiHelper.ActionFromOperationResult(operationResult);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

    }
}