using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Services
    {
        public int ServicesId { get; set; }

        [Required]
        public string ServiceName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }


    }

}
