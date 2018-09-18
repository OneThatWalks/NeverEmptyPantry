using System.Collections.Generic;
using System.Linq;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Common.Models
{
    public class AdminIndexViewModel
    {
        public string ActiveLists { get; set; }
        public string ProcessedLists { get; set; }
        public string DeliveredItems { get; set; }
        public IList<ProfileDto> Contributors { get; set; }
        public IList<UserProductVoteDto> ContributorVotes { get; set; }

        public IList<UserProductVoteDto> GetContributorVotes(string email) =>
            ContributorVotes.Where(vote => vote.ApplicationUser.Email == email).ToList();
    }
}