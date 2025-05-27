using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Providers
    {
        public int ProvidersId { get; set; }


        [Required]
        public int Age{ get; set; }
        public string? ProviderStatus { get; set; }
        public string? Description { get; set; }

        public int UsersId { get; set; }
        public Users Users { get; set; }    
        public int ServicesId { get; set; }
        public Services Services { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }


    }
}
