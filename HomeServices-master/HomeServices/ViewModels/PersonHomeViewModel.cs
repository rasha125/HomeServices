using HomeServices.Models;
using HomeServices.ViewModels;

    public class PersonHomeViewModel
    {
    public Persons PersonInfo { get; set; }
    public List<Providers> NearestProviders { get; set; }
        public List<Orders> MyOrders { get; set; }
    }

