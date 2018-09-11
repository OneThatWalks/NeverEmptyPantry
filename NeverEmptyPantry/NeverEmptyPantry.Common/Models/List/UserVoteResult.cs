using System.Collections.Generic;
using System.Linq;

namespace NeverEmptyPantry.Common.Models.List
{
    public class UserVoteResult : Result
    {
        public UserProductVoteDto UserProductVote { get; set; }

        public static UserVoteResult VoteSuccess(UserProductVoteDto vote) => new UserVoteResult
        {
            UserProductVote = vote,
            Succeeded = true
        };

        public static UserVoteResult VoteFailed(params Error[] errors) => new UserVoteResult
        {
            Errors = errors,
            Succeeded = false
        };
    }

    public class UserVotesResult : Result
    {
        public UserProductVoteDto[] UserProductVotes { get; set; }

        public static UserVotesResult VoteSuccess(params UserProductVoteDto[] vote) => new UserVotesResult
        {
            UserProductVotes = vote,
            Succeeded = true
        };

        public static UserVotesResult VoteFailed(params Error[] errors) => new UserVotesResult
        {
            Errors = errors,
            Succeeded = false
        };
    }
}