using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Persistence.Repositories
{
    public class BookItemRepository : CRUDRepository<BookingItem>, IBookingItemRepo
    {
        public BookItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<BookingItem> GetBookByIdAsync(int bookId, int resId, params Expression<Func<BookingItem, object>>[] includes)
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
            var foundEntity = await _context.Set<BookingItem>()
                .FindAsync(bookId, resId);

            if (foundEntity == null)
                return null;

            _context.Entry(foundEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return foundEntity;
        }

        public async Task<BookingItem> DeleteBookAsyn(int bookId, int resId)
        {
            var foundEntity = await _context.Set<BookingItem>()
                   .FindAsync(bookId, resId);

            if (foundEntity == null)
                return null;

            _context.Set<BookingItem>().Remove(foundEntity);
            await _context.SaveChangesAsync();

            return foundEntity;

        }
    }
}
