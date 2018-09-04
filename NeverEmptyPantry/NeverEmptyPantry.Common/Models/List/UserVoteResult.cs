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
}