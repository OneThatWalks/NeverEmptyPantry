using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models.Product
{
    public class ProductsResult : Result
    {
        public IEnumerable<ProductDto> Products { get; set; }

        public static ProductsResult ProductsSuccess(IEnumerable<ProductDto> products) => new ProductsResult
        {
            Succeeded = true,
            Products = products
        };

        public static ProductsResult ProductsFailed(Error[] errors) => new ProductsResult
        {
            Succeeded = false,
            Products = null,
            Errors = errors
        };
    }
}