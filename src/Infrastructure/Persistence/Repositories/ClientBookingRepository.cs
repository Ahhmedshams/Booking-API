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
            var serviceExisting = await _context.Set<ClientBooking>().FindAsync(serviceId);
            if (serviceExisting == null)
                return false; //service isn't exist
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

        public async Task<ClientBooking> GetBookingById(int id)
        {
            var clientBooking = await _context.Set<ClientBooking>()
                                    .Where(b => b.IsDeleted == false & b.Id == id)
                                    .FirstOrDefaultAsync();
            return clientBooking;
        }
    }
}
