using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Models
{
    public class UserProductVoteDto
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ListProductDto ListProduct { get; set; }
        public DateTime AddedDateTime { get; set; }
        public UserProductVoteState UserProductVoteState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static UserProductVoteDto From(UserProductVote userProductVote) => new UserProductVoteDto
        {
            Id = userProductVote.Id,
            ApplicationUser = userProductVote.ApplicationUser,
            ListProduct = ListProductDto.From(userProductVote.ListProduct),
            AddedDateTime = userProductVote.AddedDateTime,
            UserProductVoteState = userProductVote.UserProductVoteState,
            AuditDateTime = userProductVote.AuditDateTime
        };
    }
}