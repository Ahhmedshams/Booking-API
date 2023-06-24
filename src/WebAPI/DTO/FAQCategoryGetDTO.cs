using Microsoft.VisualBasic;

namespace WebAPI.DTO
{
    public class FAQCategoryGetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FAQGetDTO> FAQS { get; set; } = new List<FAQGetDTO>();
    }
}
