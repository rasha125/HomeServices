using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Orders
    {
        public int OrdersId { get; set; }

        [Required]
        public string Address { get; set; }

        public string? MapUrl { get; set; }

        [Required]
        public DateTime OrdersDate { get; set; }

        [Required]
        public DateTime OrdersTime { get; set; }

        public string Status { get; set; }

        public string? Description { get; set; }

        public int ProviderId { get; set; }
        public Providers Providers { get; set; }

        public int PersonId { get; set; }
        public Persons Persons { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

        
    }
}
