using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Models.Vote
{
    public class UserProductVoteModel
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ProductModel Product { get; set; }
        public DateTime AddedDateTime { get; set; }
        public UserProductVoteState UserProductVoteState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static UserProductVoteModel From(UserProductVote userProductVote) => new UserProductVoteModel
        {
            Id = userProductVote.Id,
            ApplicationUser = userProductVote.ApplicationUser,
            Product = ProductModel.FromProduct(userProductVote.Product),
            AddedDateTime = userProductVote.AddedDateTime,
            UserProductVoteState = userProductVote.UserProductVoteState,
            AuditDateTime = userProductVote.AuditDateTime
        };
    }
}