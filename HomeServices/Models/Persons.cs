namespace HomeServices.Models
{
    public class Persons
    {
        public int PersonsId { get; set; }
        public int UsersId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }

    }
}
