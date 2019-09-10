using Microsoft.EntityFrameworkCore;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeverEmptyPantry.Common.Interfaces.Repository;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Repository.Services
{
    public class ListRepository : IListRepository
    {
        private readonly ApplicationDbContext _context;

        public ListRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<List>> GetListsAsync(Func<List, bool> query)
        {
            return _context.Lists.Where(query).ToList();
        }

        public async Task<IEnumerable<List>> GetListsAsync()
        {
            return await _context.Lists.ToListAsync();
        }

        public async Task<List> GetListAsync(int id)
        {
            return await _context.Lists.SingleOrDefaultAsync(l => l.Id == id);
        }

        public async Task<ListResult> AddListAsync(ListDto list)
        {
            var listToAdd = new List
            {
                AuditDateTime = DateTime.UtcNow,
                EndDateTime = list.EndDateTime,
                Name = list.Name,
                OrderState = list.OrderState,
                StartDateTime = list.StartDateTime
            };

            try
            {
                await _context.Lists.AddAsync(listToAdd);

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListResult.ListFailed(listError);
            }

            return ListResult.ListSuccess(ListDto.From(listToAdd), null, null);
        }

        public async Task<ListResult> UpdateListAsync(int id, ListDto list)
        {
            var listToUpdate = await _context.Lists.SingleOrDefaultAsync(l => l.Id == id);

            if (listToUpdate == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {id}"
                };
                return ListResult.ListFailed(err);
            }

            listToUpdate.EndDateTime = list.EndDateTime;
            listToUpdate.StartDateTime = list.StartDateTime;
            listToUpdate.Name = list.Name;
            listToUpdate.OrderState = list.OrderState;
            listToUpdate.AuditDateTime = DateTime.UtcNow;

            _context.Lists.Update(listToUpdate);

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListResult.ListFailed(listError);
            }

            return ListResult.ListSuccess(ListDto.From(listToUpdate), null, null);
        }

        public async Task<ListResult> DeleteListAsync(int id)
        {
            var listToUpdate = await _context.Lists.SingleOrDefaultAsync(l => l.Id == id);

            if (listToUpdate == null)
            {
                var err = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkNotFoundError,
                    Description = $"Could not find list with id {id}"
                };
                return ListResult.ListFailed(err);
            }

            listToUpdate.OrderState = OrderState.LIST_REMOVED;
            listToUpdate.AuditDateTime = DateTime.UtcNow;

            _context.Lists.Update(listToUpdate);

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var listError = new OperationError
                {
                    Code = ErrorCodes.EntityFrameworkGeneralError,
                    Description = e.Message
                };
                return ListResult.ListFailed(listError);
            }

            return ListResult.ListSuccess(ListDto.From(listToUpdate), null, null);
        }
    }
}