using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Identity;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class UserListProductVote : BaseEntity
    {
        public UserProductVoteState UserProductVoteState { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Product Product { get; set; }
        public List List { get; set; }
    }
}