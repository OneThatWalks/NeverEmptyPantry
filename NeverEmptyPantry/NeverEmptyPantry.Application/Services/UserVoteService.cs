using System;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Application;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Application.Services
{
    public class UserVoteService : IUserVoteService
    {
        private readonly IUserVoteRepository _userVoteRepository;

        public UserVoteService(IUserVoteRepository userVoteRepository)
        {
            _userVoteRepository = userVoteRepository;
        }

        public async Task<UserVoteResult> CreateVote(int listProductId, ApplicationUser user, UserProductVoteState voteState)
        {
            var result = await _userVoteRepository.CreateVoteAsync(listProductId, user, voteState);

            return result;
        }

        public async Task<UserVoteResult> RemoveVote(int listProductId, ApplicationUser user)
        {
            var result = await _userVoteRepository.RemoveVoteAsync(listProductId, user);

            return result;
        }

        public async Task<UserVoteResult> UpdateVote(int id, UserProductVoteState voteState)
        {
            var result = await _userVoteRepository.UpdateVoteAsync(id, voteState);

            return result;
        }

        public async Task<UserVoteResult> GetVote(int id)
        {
            var result = await _userVoteRepository.GetVoteAsync(id);

            return result;
        }

        public async Task<UserVotesResult> Votes(Func<UserProductVote, bool> query)
        {
            var result = await _userVoteRepository.Votes(query);

            return result;
        }
    }
}