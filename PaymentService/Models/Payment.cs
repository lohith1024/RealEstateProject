using System;

namespace PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Currency { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
        public string FailureReason { get; set; }
        public int PropertyId { get; set; }

        public Payment()
        {
            PaymentMethod = string.Empty;
            Currency = "USD";
            TransactionId = string.Empty;
            FailureReason = string.Empty;
            PaymentDate = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
        }
    }
}