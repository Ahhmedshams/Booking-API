﻿namespace WebAPI.DTO
{
    public class ServiceDTO
    {
        //public int Id { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public ServiceStatus Status { get; set; }

        //public IFormFile? UploadedImage { get; set; }
    }

    //public class ServiceResDTO
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public ServiceStatus Status { get; set; }
    //    public List<string> ResoureceTypes { get; set; }
    //}
}
