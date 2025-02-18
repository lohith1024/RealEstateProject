using System.Threading.Tasks;

namespace BookingService.Interfaces
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    }
}