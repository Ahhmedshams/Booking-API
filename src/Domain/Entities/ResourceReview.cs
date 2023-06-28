using Domain.Identity;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ResourceReview : BaseEntity
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string UserId { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public int Rating { get; set; }
        public string Description { get; set; }
        public ApplicationUser User { get; set; }
        public Resource Resource { get; set; }
    }
}
