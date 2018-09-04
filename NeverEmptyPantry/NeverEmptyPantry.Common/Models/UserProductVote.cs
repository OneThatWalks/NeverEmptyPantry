using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Models
{
    public class UserProductVote
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ListProduct ListProduct { get; set; }
        public DateTime AddedDateTime { get; set; }
        public UserProductVoteState UserProductVoteState { get; set; }
        public DateTime AuditDateTime { get; set; }
    }
}