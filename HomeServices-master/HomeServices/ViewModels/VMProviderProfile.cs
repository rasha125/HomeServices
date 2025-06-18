namespace HomeServices.ViewModels
{
    public class VMProviderProfile
    {
        // From Users
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ImagePath { get; set; }


        // From Providers
        public string? Description { get; set; }
        public string? ProviderStatus { get; set; }
        public string? ServiceName { get; set; }

    }
}
