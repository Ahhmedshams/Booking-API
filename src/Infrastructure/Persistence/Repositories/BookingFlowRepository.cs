using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Models;
using Domain.Common;

namespace Infrastructure.Persistence.Repositories
{
    internal class BookingFlowRepository : CRUDRepositoryAsync<BookingFlowRepository>, IBookingFlowRepo, ISoftDeletable
    {

        public BookingFlowRepository(ApplicationDbContext context) : base(context) { }

        public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ChangeStatusToConfirmed(int BookingID)
        {
            _context.Database.ExecuteSqlRaw("EXEC SetBookingStatusConfiremed @param1",
                        new SqlParameter("@param1", BookingID)
                     );

        }

        public void ChangeStatusToCompleted(int BookingID)
        {
            _context.Database.ExecuteSqlRaw("EXEC SetBookingStatuscompleted @param1",
                        new SqlParameter("@param1", BookingID)
                    );

        }
    }
}
