using Application.Common.Helpers;
using Domain.Entities;
using Infrastructure.Persistence.Specification;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

        public async Task<decimal> PriceReport(DateTime? startDate, DateTime? endDate, int serviceId)
        {
            var report = 0.0M;
            if (serviceId == 0)
            {
                report = await _context.Set<ClientBooking>()
                               .Where(b => b.Date.Date >= startDate &&
                                      b.Date.Date <= endDate &&
                                      b.IsDeleted == false)
                               .SumAsync(b => b.TotalCost);
                return report;
            }
            report = await _context.Set<ClientBooking>()
                                .Where(b => b.ServiceId == serviceId &&
                                       b.Date.Date >= startDate &&
                                       b.Date.Date <= endDate &&
                                       b.IsDeleted == false)
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



        public async Task<int> CreateNewBooking(string userID, string date, int serviceID, string location, string startTime, string endTime, List<int> resourceID)
        {
            int resultBookingID = 0;
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    resultBookingID = 0;
                    var resultParam = new SqlParameter("@param7", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC FillClientBookingTable @param1, @param2, @param3, @param4, @param5, @param6, @param7 OUTPUT",
                        new SqlParameter("@param1", userID),
                        new SqlParameter("@param2", date),
                        new SqlParameter("@param3", serviceID),
                        new SqlParameter("@param4", location),
                        new SqlParameter("@param5", startTime),
                        new SqlParameter("@param6", endTime),
                        resultParam);

                    resultBookingID = (int)resultParam.Value;

                    int resultBookingItem = 0;
                    var resultParamBookingItem = new SqlParameter("@param2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC CheckServerTypeInBookingItem @param1, @param2 OUTPUT",
                        new SqlParameter("@param1", serviceID),
                        resultParamBookingItem);
                    resultBookingItem = (int)resultParamBookingItem.Value;

                    if (resultBookingItem == 1)
                    {
                        foreach (var item in resourceID)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1,@param2",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item)
                             );
                        }
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC FillBookingItemTableWithScheduleInvisible @param1",
                            new SqlParameter("@param1", resultBookingID)
                            );
                        await _context.Database.ExecuteSqlRawAsync(
                           "EXEC FillBookingItemTableNoScheduleInvisible @param1",
                           new SqlParameter("@param1", resultBookingID)
                           );

                    }

                    else if (resultBookingItem == 2)
                    {
                        foreach (var item in resourceID)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1,@param2",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item)
                             );
                        }
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC FillBookingItemTableWithScheduleInvisible @param1",
                            new SqlParameter("@param1", resultBookingID)
                            );

                    }
                    else if (resultBookingItem == 3)
                    {
                        foreach (var item in resourceID)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1,@param2",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item)
                             );
                        }
                        await _context.Database.ExecuteSqlRawAsync(
                         "EXEC FillBookingItemTableNoScheduleInvisible @param1",
                         new SqlParameter("@param1", resultBookingID)
                         );

                    }
                    else if (resultBookingItem == 4)
                    {
                        foreach (var item in resourceID)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1,@param2",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item)
                             );
                        }

                    }
                    else
                    {
                        return 6;
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Transaction failed. Exception: " + ex.Message);
                }
                return resultBookingID;


            }
        }
    }

}