namespace PaymentService.Models
{
    public class PaymentDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public string PaymentToken { get; set; }
        public string Description { get; set; }
        public int PropertyId { get; set; }

        public PaymentDto()
        {
            PaymentMethod = string.Empty;
            Currency = "USD";
            PaymentToken = string.Empty;
            Description = string.Empty;
        }
    }
}