using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentTransactionRepository: CRUDRepository<PaymentTransaction>, IPayemntTransactionRepository
    {
        public PaymentTransactionRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext) { }

        public async Task Refund(int paymentTransactionID)
        {
            var paymentTransaction = _context.paymentTransactions.Include(e => e.ClientBooking).FirstOrDefault(e => e.Id == paymentTransactionID);

            if (paymentTransaction != null 
                && paymentTransaction.Status == Domain.Enums.PaymentStatus.Successful
                && paymentTransaction.ClientBooking.Status != BookingStatus.Completed)
            {
                paymentTransaction.Status = Domain.Enums.PaymentStatus.Refunded;

                await _context.Database.ExecuteSqlRawAsync(
                        "EXEC CancelPendingBooking @param1",
                        new SqlParameter("@param1", paymentTransaction.ClientBookingId));

                await _context.SaveChangesAsync();
            }

        }
    }
}
