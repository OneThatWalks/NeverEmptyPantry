namespace NeverEmptyPantry.Common.Models.Vote
{
    public class ProductVoteGroup
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int VoteCount { get; set; }
        public double VotingAverage { get; set; }
        public bool IsSelected { get; set; }
    }
}