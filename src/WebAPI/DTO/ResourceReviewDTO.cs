using Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTO
{

    public class ResourceReviewDTO
    {
        public int ResourceId { get; set; }
        public string UserId { get; set; }

        [Range(1, 5,ErrorMessage ="Rate should be between 1 - 5.")]
        public int Rating { get; set; }
        public string Description { get; set; }

    }



    public record ResourceReviewResDTO(
    int id,
    int ResourceId,
    string UserId,
    int Rating,
    string Description,
    DateTime CreatedOn,
    DateTime? LastUpdatedOn
    );



}
