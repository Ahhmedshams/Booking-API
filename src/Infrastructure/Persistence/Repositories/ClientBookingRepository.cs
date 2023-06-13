using Application.Common.Helpers;
using Infrastructure.Persistence.Specification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ClientBookingRepository : CRUDRepository<ClientBooking>, IClientBookingRepo
    {
        public ClientBookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsServiceExist(int serviceId)
        {
            var service = await _context.Set<Service>()
                                    .Where(s => s.IsDeleted == false & s.Id == serviceId)
                                    .FirstOrDefaultAsync();

            if (service == null)
                return false;
            return true;
        }

        public async Task<bool> IsUserExist(string UserId)
        {
            var user = await _context.Set<ApplicationUser>()
                                    .Where(s => s.Id == UserId)
                                    .FirstOrDefaultAsync();
            if (user == null) 
                return false;
            return true;
        }

        public async Task DeleteSoft(int id)
        {
            var clientbook = await GetByIdAsync(id);
            clientbook.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClientBooking>> GetAllBookings()
        {
            var clientBookings = await _context.Set<ClientBooking>()
                                    .Where(x => x.IsDeleted == false)
                                    .ToListAsync();
            return clientBookings;
        }

        public async Task<IEnumerable<ClientBooking>> GetAllBookingsWithSpec(ISpecification<ClientBooking> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<ClientBooking> GetBookingById(int id)
        {
            var clientBooking = await _context.Set<ClientBooking>()
                                    .Where(b => b.IsDeleted == false & b.Id == id)
                                    .FirstOrDefaultAsync();
            return clientBooking;
        }

        private IQueryable<ClientBooking> ApplySpecification(ISpecification<ClientBooking> spec)
        {
            return SpecificationEvaluator<ClientBooking>.GetQuery(_context.Set<ClientBooking>(), spec);
        }

        public async Task<decimal> PriceReport(DateTime? startDate, DateTime? endDate)
        {
            var report = await _context.Set<ClientBooking>()
                                .Where(b => b.Date.Date >= startDate && b.Date.Date <= endDate && b.IsDeleted == false)
                                .SumAsync(b => b.TotalCost);
            return report;
        }

        public async Task<int> BookingsNoReport(DateTime? startDate, DateTime? endDate)
        {
            var report = await _context.Set<ClientBooking>()
                                .Where(b => b.Date.Date >= startDate && 
                                       b.Date.Date <= endDate && 
                                       b.Status == BookingStatus.Completed &&
                                       b.IsDeleted == false)
                                .CountAsync();
            return report;
        }

        public async Task<int> CancelledBookingsReport(DateTime? startDate, DateTime? endDate)
        {
            var report = await _context.Set<ClientBooking>()
                                .Where(b => b.Date.Date >= startDate && b.Date.Date <= endDate &&
                                       b.Status == BookingStatus.Cancelled && b.IsDeleted == false)
                                .CountAsync();
            return report;
        }
    }
}
