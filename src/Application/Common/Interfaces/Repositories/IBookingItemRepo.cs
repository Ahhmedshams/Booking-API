using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBookingItemRepo: IAsyncRepository<BookingItem>
    {
        Task<BookingItem> GetBookByIdAsync(int bookId, int resId, params Expression<Func<BookingItem, object>>[] includes);

        Task<BookingItem> EditBookAsyn(int bookId, int resId, BookingItem entity);
        Task<BookingItem> DeleteBookAsyn(int bookId, int resId);
    }
}
