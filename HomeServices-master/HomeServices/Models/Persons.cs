using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Persons
    {
        public int PersonsId { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public Users User { get; set; }    

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }


  

    }
}
