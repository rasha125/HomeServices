namespace HomeServices.Models
{
    public class Messages
    {
        public int MessagesId { get; set; }

        public int SenderId { get; set; }

        public int ReceiverId { get; set; }

        public Users Users { get; set; }

        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

    }
}
