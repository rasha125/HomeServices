using HomeServices.Models;

namespace HomeServices.ViewModels
{
    public class VMPersonIndex
    {
        public Persons PersonInfo { get; set; }
        public List<Providers> NearbyProviders { get; set; }

        public List<Orders> MyOrders { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<int> RatedOrderIds { get; set; } = new List<int>();
    }
}
