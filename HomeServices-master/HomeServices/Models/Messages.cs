using System.ComponentModel.DataAnnotations;

namespace HomeServices.Models
{
    public class Messages
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }

        public Users Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public Users Receiver { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
