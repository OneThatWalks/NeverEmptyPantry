using NeverEmptyPantry.Common.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeverEmptyPantry.Common.Interfaces.Repository
{
    public interface IUserVoteRepository
    {
        /// <summary>
        /// Creates a vote
        /// </summary>
        /// <param name="userProductVote">The user product vote to create</param>
        /// <returns>A task result that represents the created vote</returns>
        Task<UserProductVote> CreateVoteAsync(UserProductVote userProductVote);

        /// <summary>
        /// Updates a user product vote
        /// </summary>
        /// <param name="userProductVote">The user product vote to update</param>
        /// <returns>A task result that represents the updated vote</returns>
        Task<UserProductVote> UpdateVoteAsync(UserProductVote userProductVote);

        /// <summary>
        /// Removes a vote
        /// </summary>
        /// <param name="userProductVote">The user product vote to remove</param>
        /// <returns>A task result that represents the removed vote</returns>
        Task<UserProductVote> RemoveVoteAsync(UserProductVote userProductVote);

        /// <summary>
        /// Gets the votes for a list
        /// </summary>
        /// <param name="listId">The list to query votes for</param>
        /// <returns>A task result that represents votes</returns>
        Task<IEnumerable<UserProductVote>> GetListProductVotesAsync(int listId);

        /// <summary>
        /// Gets a vote
        /// </summary>
        /// <param name="id">The id of the vote</param>
        /// <returns>A task result that represents the vote</returns>
        Task<UserProductVote> GetVoteAsync(int id);

        /// <summary>
        /// Gets all votes that match a query
        /// </summary>
        /// <param name="query">The query function</param>
        /// <returns>A task result that represents the matching votes</returns>
        Task<IEnumerable<UserProductVote>> Votes(Func<UserProductVote, bool> query);
    }
}