using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Identity;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Vote;
using NeverEmptyPantry.Repository.Entity;

namespace NeverEmptyPantry.Repository.Services
{
    public class UserVoteRepository : IUserVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public UserVoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserVoteResult> CreateVoteAsync(int listProductId, ApplicationUser user, UserProductVoteState voteState)
        {
            var userProductVote = await _context.UserProductVotes.SingleOrDefaultAsync(x =>
                x.ListProduct.Id == listProductId && x.ApplicationUser == user);

            if (userProductVote != null)
            {
                // Vote exists just update it
                return await UpdateVoteAsync(userProductVote.Id, voteState);
            }

            var listProduct = await _context.ListProducts.SingleOrDefaultAsync(y => y.Id == listProductId);

            if (listProduct == null)
            {
                var error = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"List Product with id {listProductId} not found."
                };
                return UserVoteResult.VoteFailed(error);
            }

            var vote = new UserProductVote
            {
                ApplicationUser = user,
                ListProduct = listProduct,
                AddedDateTime = DateTime.UtcNow,
                UserProductVoteState = voteState
            };

            var result = await _context.UserProductVotes.AddAsync(vote);

            await _context.SaveChangesAsync();

            return UserVoteResult.VoteSuccess(UserProductVoteDto.From(result.Entity));
        }

        public async Task<UserVoteResult> UpdateVoteAsync(int id, UserProductVoteState voteState)
        {
            var userProductVote = await _context.UserProductVotes.Include(upv => upv.ApplicationUser).Include(upr => upr.ListProduct).ThenInclude(lp => lp.List).Include(upr => upr.ListProduct).ThenInclude(lp => lp.Product).SingleOrDefaultAsync(x =>
                x.Id == id);

            userProductVote.AuditDateTime = DateTime.UtcNow;
            userProductVote.UserProductVoteState = voteState;

            await _context.SaveChangesAsync();

            return UserVoteResult.VoteSuccess(UserProductVoteDto.From(userProductVote));
        }

        public async Task<UserVoteResult> RemoveVoteAsync(int listProductId, ApplicationUser user)
        {
            var userProductVote = await _context.UserProductVotes.Include(vote => vote.ApplicationUser).Include(vote => vote.ListProduct).ThenInclude(lp => lp.List).Include(vote => vote.ListProduct).ThenInclude(lp => lp.Product).SingleOrDefaultAsync(x =>
                x.ListProduct.Id == listProductId && x.ApplicationUser == user);

            if (userProductVote == null)
            {
                var error = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"User Vote for {user.FirstName} not found."
                };
                return UserVoteResult.VoteFailed(error);
            }

            _context.Update(userProductVote);

            userProductVote.UserProductVoteState = UserProductVoteState.VOTE_REMOVED;
            userProductVote.AuditDateTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return UserVoteResult.VoteSuccess(UserProductVoteDto.From(userProductVote));
        }

        public async Task<IEnumerable<UserProductVoteDto>> GetListProductVotesAsync(int listId)
        {
            var listProducts = await _context.ListProducts.Include("List").Where(l => l.List.Id == listId).Select(i => i.Id).ToListAsync();

            var result = await _context.UserProductVotes.Include(vote => vote.ListProduct).ThenInclude(lp => lp.List).Include(vote => vote.ApplicationUser).Where(upv => upv.ListProduct.List.Id == listId).ToListAsync();

            return result?.Select(UserProductVoteDto.From).ToList();
        }

        public async Task<UserVoteResult> GetVoteAsync(int id)
        {
            var result = await _context.UserProductVotes
                .Include(vote => vote.ListProduct).ThenInclude(lp => lp.List)
                .Include(lp => lp.ListProduct).ThenInclude(lp => lp.Product)
                .Include(vote => vote.ApplicationUser)
                .SingleOrDefaultAsync(v => v.Id == id);

            if (result == null)
            {
                return UserVoteResult.VoteFailed(new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Vote with id {id} not found"
                });
            }

            return UserVoteResult.VoteSuccess(UserProductVoteDto.From(result));
        }

        public async Task<UserVotesResult> Votes(Func<UserProductVote, bool> query)
        {
            var result = _context.UserProductVotes
                .Include(vote => vote.ApplicationUser)
                .Include(vote => vote.ListProduct).ThenInclude(lp => lp.List)
                .Include(vote => vote.ListProduct).ThenInclude(lp => lp.Product)
                .Where(query)
                .ToArray();

            if (result.Length == 0)
            {
                return UserVotesResult.VoteFailed(new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"No votes returned for query"
                });
            }

            return UserVotesResult.VoteSuccess(result.Select(UserProductVoteDto.From).ToArray());
        }
    }
}