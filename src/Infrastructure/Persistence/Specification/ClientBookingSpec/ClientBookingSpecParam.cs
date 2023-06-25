using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Specification.ClientBookingSpec
{
    public class ClientBookingSpecParam: PagingParams
    {
        public string? Sort { get; set; }
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public BookingStatus? Status { get; set; }
        //public int? ServiceId { get; set; }
        //public string? UserEmail { get; set; }
		public string? UserID { get; set; }
	}
}
