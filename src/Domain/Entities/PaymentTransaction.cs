using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PaymentTransaction : BaseEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int ClientBookingId { get; set; }

        public int PaymentMethodId { get; set; }

        public string? TransactionId { get; set; }

        public string SessionId { get; set; }

        public PaymentStatus  Status { get; set; }

        public decimal Amount { get; set; }

        public string? Error { get; set; }

        public ClientBooking ClientBooking { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public ApplicationUser User { get; set; }
    }
}
