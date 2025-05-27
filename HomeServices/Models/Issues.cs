using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Issues
    {
        public int IssuesId { get; set; }

        [Required]
        public string Type { get; set; }
        public string ?Description { get; set; }
        public string? File { get; set; }
        public int UserId { get; set; }
        public Users Users{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }



    }
}
