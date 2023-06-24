using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class FAQ:BaseEntity
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public FAQCategory FAQCategory { get; set; }
        public int FAQCategoryId { get; set; }
    }
}
