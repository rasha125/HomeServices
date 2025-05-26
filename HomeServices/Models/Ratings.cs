namespace HomeServices.Models
{
    public class Ratings
    {
        public int RatingsId { get; set; }

        public string? Comment { get; set; }

        public int RatingValue { get; set; } // Assuming a rating value between 1 and 5

        public int OrdersId { get; set; }
        public int PersonsId { get; set; } // User who provided the rating
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }
    }
}
