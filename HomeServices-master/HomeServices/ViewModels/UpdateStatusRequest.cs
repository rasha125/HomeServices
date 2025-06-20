namespace HomeServices.ViewModels
{
    // DTO for updating the status of an order
    public class UpdateStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
