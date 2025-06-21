using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using HomeServices.Enums;

namespace HomeServices.Models
{
    public class Issues
    {
        public int IssuesId { get; set; }

        [Required]
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? File { get; set; }
        public string UserId { get; set; }
        public Users User{ get; set; }
        public ReportStatus Status { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }



    }
}
