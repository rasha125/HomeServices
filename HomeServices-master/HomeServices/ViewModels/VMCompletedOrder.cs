namespace HomeServices.ViewModels
{
    public class VMCompletedOrder
    {
        public int OrderId { get; set; }
        public DateTime OrdersDate { get; set; }
        public TimeSpan OrdersTime { get; set; }
        public string ProviderName { get; set; }
        public string PersonName { get; set; }
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string MapUrl { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        // ✅ تقييم الطلب إن وجد
        public int? RatingValue { get; set; }
        public string? RatingComment { get; set; }
    }
}
