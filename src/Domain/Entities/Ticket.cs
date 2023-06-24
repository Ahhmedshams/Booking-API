using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser?  ApplicationUser { get; set; }
        public string? TicketDescription { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public Ticket()
        {
            CreatedAt  = DateTime.Now;
        }
        public string? RecivedAdminId { get; set; }
        public DateTime? AdminRecivedAt { get; set; }
        public DateTime? ContactDoneAt { get; set; }

        // Relation Table 
        //public virtual ICollection<TicketReviewed> TicketReviewed { get; set; }

    }
}
