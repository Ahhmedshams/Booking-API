﻿using Application.Common.Helpers;
using Infrastructure.Persistence.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class BookItemRepository : CRUDRepository<BookingItem>, IBookingItemRepo
    {
        public BookItemRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<BookingItem> GetBookByComplexIdsAsync(int bookId, int resId, params Expression<Func<BookingItem, object>>[] includes)
        {
            var query = _context.Set<BookingItem>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await _context.Set<BookingItem>().FindAsync(bookId, resId);
        }


        public async Task<BookingItem> EditBookAsyn(int bookId, int resId, BookingItem entity)
        {
            var bookingItem = await GetBookByComplexIdsAsync(bookId, resId);

            if (bookingItem == null)
                return null;

            await DeleteBookAsyn(bookId, resId);
            await AddAsync(entity);

            return bookingItem;
        }

        public async Task<BookingItem> DeleteBookAsyn(int bookId, int resId)
        {
            var bookingItem = await _context.Set<BookingItem>()
                   .FindAsync(bookId, resId);

            if (bookingItem == null)
                return null;

            _context.Set<BookingItem>().Remove(bookingItem);
            await _context.SaveChangesAsync();

            return bookingItem;

        }

        public async Task<bool> CheckDuplicateKey(int bookId, int resId)
        {
            var bookingItem = await GetBookByComplexIdsAsync(bookId, resId);
            if(bookingItem == null)
                return false;
            return true; 
        }

        public async Task<IEnumerable<BookingItem>> GetBookItemByIdAsync(int bookId, params Expression<Func<BookingItem, object>>[] includes)
        {
            var bookingItems = await _context.Set<BookingItem>()
                                    .Where(b => b.BookingId == bookId)
                                    .ToListAsync();
            if (bookingItems.Count() == 0)
                return null;

            return bookingItems;
        }

        public async Task DeleteBulk(int bookId)
        {
            await _context.Set<BookingItem>().Where(b => b.BookingId == bookId).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookingItem>> AddRange(IEnumerable<BookingItem> bookingItems)
        {
            await _context.Set<BookingItem>().AddRangeAsync(bookingItems);
            await _context.SaveChangesAsync();
            return bookingItems;

        }

        public async Task<bool> IsClientBookExis(int bookId)
        {
            var clientBookExist = await _context.Set<ClientBooking>().FindAsync(bookId);
            if (clientBookExist == null)
                return false;
            return true;
        }

        public async Task<bool> IsResourecExist(int resId)
        {
            var resourceExist = await _context.Set<Resource>().FindAsync(resId);
            if (resourceExist == null)
                return false;
            return true;
        }


        public async Task<IEnumerable<BookingItem>> GetAllBooksWithSpec(ISpecification<BookingItem> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        private IQueryable<BookingItem> ApplySpecification(ISpecification<BookingItem> spec)
        {
            return SpecificationEvaluator<BookingItem>.GetQuery(_context.Set<BookingItem>(), spec);
        }


    }
}
