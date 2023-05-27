using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBookingItemRepo: IAsyncRepository<BookingItem>
    {
        Task<IEnumerable<BookingItem>> AddBulk(IEnumerable<BookingItem> bookingItems);
        Task<BookingItem> GetBookByComplexIdsAsync(int bookId, int resId, params Expression<Func<BookingItem, object>>[] includes);
        Task<IEnumerable<BookingItem>> GetBookItemByIdAsync(int bookId, params Expression<Func<BookingItem, object>>[] includes);
        Task<BookingItem> EditBookAsyn(int bookId, int resId, BookingItem entity);
        Task<BookingItem> DeleteBookAsyn(int bookId, int resId);
        Task DeleteBulk(int bookId);
        Task<bool> IsClientBookExis(int bookId);
        Task<bool> IsResourecExist(int resId);
        Task<bool> CheckDuplicateKey(int bookId, int resId);
    }
}
