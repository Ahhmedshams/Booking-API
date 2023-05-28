using Application.Common.Helpers;

namespace Application.Common.Interfaces.Repositories
{
    public interface IClientBookingRepo: IAsyncRepository<ClientBooking>
    {
        Task<IEnumerable<ClientBooking>> GetAllBookingsWithSpec(ISpecification<ClientBooking> spec);
        Task<IEnumerable<ClientBooking>> GetAllBookings();
        Task<ClientBooking> GetBookingById(int id);
        Task<bool> IsServiceExist(int serviceId);
        Task DeleteSoft(int id);
    }
}
