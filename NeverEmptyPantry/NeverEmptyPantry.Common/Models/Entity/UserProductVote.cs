using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class UserProductVote
    {
        public int Id { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime AuditDateTime { get; set; }
        public UserProductVoteState UserProductVoteState { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Product Product { get; set; }
    }
}