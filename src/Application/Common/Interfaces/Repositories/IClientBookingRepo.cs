﻿using Application.Common.Helpers;

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
        Task<int> CreateNewBooking(string userID, string date, int serviceID, string location, string startTime, string endTime,List<int> resourceID);

        Task CancelBooking(int bookingID);


        //Task<ClientBooking> FillBookingItem(int bookingId, List<int> resourceID);


        Task<IEnumerable<ClientBooking>> GetUserBooking(string id);

        Task<ClientBooking> GetUserBooking(string id,int bookingId);
        Task CompleteBooking(int bookingID);
    }
}
