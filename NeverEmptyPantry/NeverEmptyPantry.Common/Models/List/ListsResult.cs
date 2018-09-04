using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListsResult : Result
    {
        public IEnumerable<ListDto> Lists { get; set; }

        public static ListsResult ListsSuccess(IEnumerable<ListDto> lists) => new ListsResult
        {
            Lists = lists,
            Succeeded = true
        };

        public static ListsResult ListsFailure(params Error[] errors) => new ListsResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}