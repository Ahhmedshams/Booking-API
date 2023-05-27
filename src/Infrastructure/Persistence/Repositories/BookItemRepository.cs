using Application.Common.Interfaces.Repositories;
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
            var foundEntity = await GetBookByIdAsync(bookId, resId);

            if (foundEntity == null)
                return null;

            await DeleteBookAsyn(bookId, resId);
            await AddAsync(entity);

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

        public async Task<int> CheckExistenceOfBookIdAndResId(int bookId, int resId)
        {
            var clientBookExist = await _context.Set<ClientBooking>().FindAsync(bookId);
            if (clientBookExist == null)
                return 1; //Client book not exist

            var resourceExist = await _context.Set<Resource>().FindAsync(resId);
            if (resourceExist == null)
                return -1; //Resource not exist

            return 0; //both are found
        }

        public async Task<bool> CheckDuplicateKey(int bookId, int resId)
        {
            var objectExist = await GetBookByIdAsync(bookId, resId);
            if(objectExist == null)
                return false;
            return true; 
        }
    }
}
