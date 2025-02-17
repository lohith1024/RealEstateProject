using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentService.Models;

namespace PaymentService.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPayment(PaymentDto paymentDto);
        Task<bool> RefundPayment(int paymentId);
        Task<PaymentStatus> CheckPaymentStatus(int paymentId);
        Task<List<Transaction>> ListTransactions(int userId);
        Task<List<Payment>> GetAllPayments();
        Task<Payment?> GetPaymentById(int paymentId);
        Task<bool> UpdatePayment(int id, PaymentDto paymentDto);
        Task<bool> DeletePayment(int id);
    }
}