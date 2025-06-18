using HomeServices.Models;

namespace HomeServices.ViewModels
{
    public class VMPersonIndex
    {
        public List<Persons> People { get; set; }
        public List<Providers> NearbyProviders { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
