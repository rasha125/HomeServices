using System.ComponentModel.DataAnnotations;
namespace HomeServices.ViewModels
{
    public class VMIssue
    {
        [Required]
        public string Type { get; set; }

        public string? Description { get; set; }

        public IFormFile? File { get; set; }
    }
}
