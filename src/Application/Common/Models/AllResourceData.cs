namespace Application.Common.Model
{
    public class AllResourceData
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public double AverageRating { get; set; }
        public int NumRatings { get; set; }
        public ICollection<ResourceImage> Images { get; set; }

        public List<Attribute> Attributes { get; set; } = new List<Attribute>();
    }


    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
