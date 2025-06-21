using HomeServices.Models;

namespace HomeServices.ViewModels
{
    public class ServiceProvidersViewModel
    {
        public string ServiceName { get; set; }

        public List<ProviderWithRatingViewModel> ProvidersWithRatings { get; set; }

        public int PersonId { get; set; }
    }
}
