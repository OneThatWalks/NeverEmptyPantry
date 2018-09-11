using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Interfaces
{
    public interface IUserVoteRepository
    {
        Task<UserVoteResult> CreateVoteAsync(int listProductId, ApplicationUser user, UserProductVoteState voteState);
        Task<UserVoteResult> UpdateVoteAsync(int id, UserProductVoteState voteState);
        Task<UserVoteResult> RemoveVoteAsync(int listProductId, ApplicationUser user);
        Task<IEnumerable<UserProductVoteDto>> GetListProductVotesAsync(int listId);
        Task<UserVoteResult> GetVoteAsync(int id);
        Task<UserVotesResult> Votes(Func<UserProductVote, bool> query);
    }
}