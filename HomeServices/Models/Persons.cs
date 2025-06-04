namespace HomeServices.Models
{
    public class Persons
    {
        public int PersonsId { get; set; }
        public int UsersId { get; set; }
        public Users Users { get; set; }    
    }
}
