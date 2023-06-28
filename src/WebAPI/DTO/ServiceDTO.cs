using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ServiceDTO
    {
        public int? Id { get; set;}
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ServiceStatus Status { get; set; }

        public ICollection<IFormFile>? UploadedImages { get; set; }
    }


    public class ServiceResDTO
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ServiceStatus Status { get; set; }
        //public List<string> ResoureceTypes { get; set; }
        public List<string> ImageUrls { get; set; }
	}
}
