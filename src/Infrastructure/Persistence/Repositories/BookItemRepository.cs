using Application.Common.Helpers;
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

        public async Task<IEnumerable<BookingItem>> AddBulk(IEnumerable<BookingItem> bookingItems)
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

        public async Task<IEnumerable<dynamic>> TopResourcesReport(DateTime startDate, DateTime endDate, int number)
        {
            if (number == 0)
                number = 5;
            var top5Resources = await _context.Set<BookingItem>()
                            .Where(b => b.ClientBooking.Date.Date >= startDate.Date &&
                                        b.ClientBooking.Date.Date <= endDate.Date &&
                                        b.IsDeleted == false)
                            .GroupBy(b => new { b.Resource.Name })
                            .Select(g => new
                            {
                                ResourceName = g.Key.Name,
                                TotalPrice = g.Sum(b => b.Price)
                            })
                            .OrderByDescending(r => r.TotalPrice)
                            .Take(number)
                            .ToListAsync();

            return top5Resources;
        }

        public async Task<IEnumerable<dynamic>> ResourceTypeBookingsReport(DateTime startDate, DateTime endDate)
        {
            var report = await _context.Set<BookingItem>()
                            .Where(b => b.ClientBooking.Date.Date >= startDate.Date &&
                                        b.ClientBooking.Date.Date <= endDate.Date &&
                                        b.IsDeleted == false)
                            .GroupBy(b => b.Resource.ResourceType.Name)
                            .Select(g => new
                            {
                                ResourceTypeName = g.FirstOrDefault().Resource.ResourceType.Name,
                                UsageCount = g.Count()
                            })
                            .OrderByDescending(r => r.UsageCount)
                            .ToListAsync();
            return report;
        }

        public async Task<IEnumerable<dynamic>> ResTypeSoldPerMonthReport(DateTime startDate, DateTime endDate)
        {
            var report = await _context.Set<BookingItem>()
                            .Where(b => b.ClientBooking.Date.Date >= startDate.Date &&
                                        b.ClientBooking.Date.Date <= endDate.Date &&
                                        b.IsDeleted == false)
                            .GroupBy(b => new
                            {
                                ResourcTypeName = b.Resource.ResourceType.Name,
                                Month = b.ClientBooking.Date.Month
                            })
                            .Select(g => new
                            {
                                ResourceType = g.Key.ResourcTypeName,
                                Month = g.FirstOrDefault().ClientBooking.Date.Month,
                                TotalPrice = g.Sum(b=> b.Price),
                                UsageCount = g.Count()
                            })
                            .OrderBy(r => r.ResourceType)
                            .ToListAsync();
            return report;
        }
    }
}
