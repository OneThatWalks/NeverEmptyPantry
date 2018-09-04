using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListResult : Result
    {
        public ListDto List { get; set; }
        public IEnumerable<ListProductDto> ListProducts { get; set; }
        public IEnumerable<UserProductVoteDto> UserProductVotes { get; set; }

        public static ListResult ListSuccess(ListDto list, IEnumerable<ListProductDto> listProducts, IEnumerable<UserProductVoteDto> userProductVotes) => new ListResult
        {
            List = list,
            ListProducts = listProducts,
            UserProductVotes = userProductVotes,
            Succeeded = true
        };

        public static ListResult ListFailed(params Error[] errors) => new ListResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}