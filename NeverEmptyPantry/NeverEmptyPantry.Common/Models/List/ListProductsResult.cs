using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductsResult : Result
    {
        public IEnumerable<ListProductDto> ListProducts { get; set; }

        public static ListProductsResult ListProductsSuccess(params ListProductDto[] products) => new ListProductsResult
        {
            Succeeded = true,
            ListProducts = products
        };

        public static ListProductsResult ListProductsFailed(params Error[] errors) => new ListProductsResult
        {
            Succeeded = true,
            Errors = errors
        };
    }
}