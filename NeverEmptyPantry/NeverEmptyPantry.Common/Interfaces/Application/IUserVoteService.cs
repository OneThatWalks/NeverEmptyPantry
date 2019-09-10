using System;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Interfaces.Application
{
    public interface IUserVoteService
    {
        Task<UserVoteResult> CreateVote(int listProductId, ApplicationUser user, UserProductVoteState voteState);
        Task<UserVoteResult> RemoveVote(int listProductId, ApplicationUser user);
        Task<UserVoteResult> UpdateVote(int id, UserProductVoteState voteState);
        Task<UserVoteResult> GetVote(int id);

        Task<UserVotesResult> Votes(Func<UserProductVote, bool> query);
    }
}