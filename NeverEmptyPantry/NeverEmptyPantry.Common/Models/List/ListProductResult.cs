using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductResult : Result
    {
        public ListProductDto ListProduct { get; set; }
        public IEnumerable<UserProductVoteDto> UserProductVotes { get; set; }

        public static ListProductResult ListProductSuccess(ListProductDto listProduct, IEnumerable<UserProductVoteDto> userProductVotes) => new ListProductResult
        {
            ListProduct = listProduct,
            UserProductVotes = userProductVotes,
            Succeeded = true
        };

        public static ListProductResult ListProductFailed(params Error[] errors) => new ListProductResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}