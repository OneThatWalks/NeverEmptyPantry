namespace NeverEmptyPantry.Common.Models.Product
{
    public class ProductResult : Result
    {
        public ProductDto Product { get; set; }

        public static ProductResult ProductSuccess(ProductDto product) => new ProductResult
        {
            Product = product,
            Succeeded = true
        };

        public static ProductResult ProductFailed(params Error[] errors) => new ProductResult
        {
            Errors = errors,
            Succeeded = false
        };
    }
}