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
        public string? Location { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public int? ServiceId { get; set; }
        public string? UserId { get; set; }
    }
}
