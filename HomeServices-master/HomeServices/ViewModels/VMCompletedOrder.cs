namespace HomeServices.ViewModels
{
    public class VMCompletedOrder
    {
        public int OrderId { get; set; }
        public string Address { get; set; }
        public string? MapUrl { get; set; }
        public DateTime OrdersDate { get; set; }
        public DateTime OrdersTime { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }

        // الأسماء بدلاً من الـ IDs
        public string ProviderName { get; set; }
        public string PersonName { get; set; }
        public string ServiceName { get; set; }
    }
}
