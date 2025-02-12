namespace POS.Models.Orders
{
    public class OrderHistoryDto
    {
        public int OrderId { get; set; }
        public string EmployeeName { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public double AmountToPay { get; set; }
        public string PaymentMethod { get; set; }

    }
}
