using Application.Common.Helpers;
using Domain.Entities;
using Infrastructure.Persistence.Specification;
using Microsoft.AspNetCore.Mvc;
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
                        int result = 2;
                        foreach (var item in resourceID)
                        {
                            var result1Param = new SqlParameter("@param3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            await _context.Database.ExecuteSqlRawAsync(
                                "EXEC FillBookingItemTableWithScheduleShown @param1, @param2, @param3 OUTPUT",
                                new SqlParameter("@param1", resultBookingID),
                                new SqlParameter("@param2", item),
                                result1Param // pass the SqlParameter instance for output parameter
                            );
                            result = (int)result1Param.Value;
                        }
                        var result2Param = new SqlParameter("@param2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC FillBookingItemTableWithScheduleInvisible @param1 ,@param2 OUTPUT",
                            new SqlParameter("@param1", resultBookingID),
                            result2Param
                        );

                        var result2 = (int)result2Param.Value;

                        var result3Param = new SqlParameter("@param2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC FillBookingItemTableNoScheduleInvisible @param1 , @param2 OUTPUT",
                            new SqlParameter("@param1", resultBookingID),
                            result3Param);
                        var result3 = (int)result3Param.Value;

                        if (result == 0 || result2 == 0 || result3 == 0)
                        {
                            await transaction.RollbackAsync();
                            return -1;
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                            "EXEC CalculateTotalCost @param1",
                            new SqlParameter("@param1", resultBookingID));

                            await transaction.CommitAsync();
                        }
                    }

                    else if (resultBookingItem == 2)
                    {
                        int result = 2;
                        foreach (var item in resourceID)
                        {
                            var result1Param = new SqlParameter("@param3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1,@param2 @param3 OUTPUT",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item),
                             result1Param);
                            result = (int)result1Param.Value;
                        }
                        var result2Param = new SqlParameter("@param2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        await _context.Database.ExecuteSqlRawAsync(
                            "EXEC FillBookingItemTableWithScheduleInvisible @param1 ,@param2 OUTPUT",
                            new SqlParameter("@param1", resultBookingID),
                            result2Param);
                        var result2 = (int)result2Param.Value;

                        if (result == 0 || result2 == 0)
                        {
                            await transaction.RollbackAsync();
                            return -1;
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC CalculateTotalCost @param1",
                             new SqlParameter("@param1", resultBookingID));

                            await transaction.CommitAsync();
                        }
                    }
                    else if (resultBookingItem == 3)
                    {
                        int result = 2;
                        foreach (var item in resourceID)
                        {
                            var result1Param = new SqlParameter("@param3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1, @param2,@param3 OUTPUT",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item),
                             result1Param);
                            result = (int)result1Param.Value;
                        }
                        var result2Param = new SqlParameter("@param2", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        await _context.Database.ExecuteSqlRawAsync(
                         "EXEC FillBookingItemTableNoScheduleInvisible @param1,@param2 OUTPUT",
                         new SqlParameter("@param1", resultBookingID),
                         result2Param);
                       var result2  = (int)result2Param.Value;

                        if (result == 0 || result2 == 0)
                        {
                            await transaction.RollbackAsync();
                            return -1;
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                            "EXEC CalculateTotalCost @param1",
                            new SqlParameter("@param1", resultBookingID));

                            await transaction.CommitAsync();
                        }

                    }
                    else if (resultBookingItem == 4)
                    {
                        int result = 2;
                        foreach (var item in resourceID)
                        {
                            var result1Param = new SqlParameter("@param3", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            await _context.Database.ExecuteSqlRawAsync(
                             "EXEC FillBookingItemTableWithScheduleShown @param1, @param2 ,@param3 OUTPUT",
                             new SqlParameter("@param1", resultBookingID),
                             new SqlParameter("@param2", item),
                             result1Param);
                            result = (int)result1Param.Value;
                        }
                        if (result == 0 )
                        {
                            await transaction.RollbackAsync();
                            return -1;
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                            "EXEC CalculateTotalCost @param1",
                            new SqlParameter("@param1", resultBookingID));

                            await transaction.CommitAsync();
                        }

                    }
                  
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return -1;
                }
                return resultBookingID;
            }
        }

    }

}

