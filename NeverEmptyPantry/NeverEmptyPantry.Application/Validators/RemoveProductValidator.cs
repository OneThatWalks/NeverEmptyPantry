using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Application.Validators
{
    public class RemoveProductValidator : IValidator<Product>
    {
        public IOperationResult Validate(Product obj)
        {
            return OperationResult.Success;
        }
    }
}