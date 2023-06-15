using Application.Common.Helpers;

namespace Application.Common.Interfaces.Repositories
{
    public interface IClientBookingRepo: IAsyncRepository<ClientBooking>
    {
        Task<IEnumerable<ClientBooking>> GetAllBookingsWithSpec(ISpecification<ClientBooking> spec);
        Task<decimal> PriceReport(DateTime? startDate, DateTime? endDate, int serviceId);
        Task<int> CancelledBookingsReport(DateTime? startDate, DateTime? endDate);
        Task<int> BookingsNoReport(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<ClientBooking>> GetAllBookings();
        Task<ClientBooking> GetBookingById(int id);
        Task<bool> IsServiceExist(int serviceId);
        Task<bool> IsUserExist(string UserId);
        Task DeleteSoft(int id);
    }
}
