﻿using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO 
{
    public class ResourceTypeDTO
    {
       // public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Shown { get; set; }
        public bool HasSchedual { get; set; }
        public ICollection<IFormFile>? UploadedImages { get; set; }


    }




}
