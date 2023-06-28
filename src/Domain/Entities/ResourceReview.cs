using Domain.Identity;

namespace Domain.Entities
{
    public class ResourceReview : BaseEntity
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public ApplicationUser User { get; set; }
        public Resource Resource { get; set; }
    }
}
