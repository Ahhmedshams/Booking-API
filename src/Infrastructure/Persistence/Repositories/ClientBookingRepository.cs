namespace Infrastructure.Persistence.Repositories
{
    public class ClientBookingRepository : CRUDRepository<ClientBooking>, IClientBookingRepo
    {
        public ClientBookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckServiceExistence(int serviceId)
        {
            var serviceExisting = await _context.Set<ClientBooking>().FindAsync(serviceId);
            if (serviceExisting == null)
                return false; //service isn't exist
            return true;
        }
    }
}
