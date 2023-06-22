using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Repositories
{
    public interface IBookingFlowRepo
    {
        public void ChangeStatusToConfirmed(int BookingID);
        public void ChangeStatusToCompleted(int BookingID);

    }
}
