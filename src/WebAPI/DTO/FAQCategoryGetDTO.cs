using Microsoft.VisualBasic;
using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class FAQCategoryGetDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public IEnumerable<FAQGetDTO> FAQS { get; set; } = new List<FAQGetDTO>();
    }
}
