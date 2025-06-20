using HomeServices.Models;

namespace HomeServices.ViewModels
{
    public class VMProviderDashboard
    {
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public int CancelledCount { get; set; }

        public List<Orders> RecentOrders { get; set; }

        public string Status { get; set; }
    }
}
