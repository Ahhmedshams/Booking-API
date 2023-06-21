namespace Domain.Entities
{
    public class FAQ:BaseEntity
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
