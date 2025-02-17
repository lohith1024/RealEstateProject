using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using PaymentService.Models;
using PaymentService.Interfaces;
using PaymentService.Data;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"] ??
                throw new InvalidOperationException("Stripe:SecretKey configuration is missing");
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payment?> GetPaymentById(int paymentId)
        {
            return await _context.Payments.FindAsync(paymentId);
        }

        public async Task<Payment> ProcessPayment(PaymentDto paymentDto)
        {
            if (paymentDto == null)
                throw new ArgumentNullException(nameof(paymentDto));

            if (string.IsNullOrEmpty(paymentDto.PaymentToken))
                throw new ArgumentException("Payment token is required", nameof(paymentDto));

            var payment = new Payment
            {
                BookingId = paymentDto.BookingId,
                UserId = paymentDto.UserId,
                Amount = paymentDto.Amount,
                PaymentMethod = paymentDto.PaymentMethod,
                Currency = paymentDto.Currency,
                PaymentDate = DateTime.UtcNow,
                Status = PaymentStatus.Processing,
                PropertyId = paymentDto.PropertyId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(paymentDto.Amount * 100), // Convert to cents
                    Currency = paymentDto.Currency.ToLower(),
                    PaymentMethod = paymentDto.PaymentToken,
                    Confirm = true,
                    Description = paymentDto.Description ?? $"Payment for Booking {paymentDto.BookingId}"
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                if (paymentIntent.Status == "succeeded")
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.TransactionId = paymentIntent.Id;
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = paymentIntent.LastPaymentError?.Message ?? "Payment processing failed";
                }
            }
            catch (StripeException ex)
            {
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = ex.Message;
            }

            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            // Create transaction record
            var transaction = new Models.Transaction
            {
                PaymentId = payment.Id,
                UserId = payment.UserId,
                Amount = payment.Amount,
                TransactionType = payment.Status == PaymentStatus.Completed ? "Payment" : "Failed Payment",
                TransactionDate = DateTime.UtcNow,
                Status = payment.Status.ToString(),
                Description = paymentDto.Description ?? $"Payment for Booking {paymentDto.BookingId}"
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<bool> UpdatePayment(int id, PaymentDto paymentDto)
        {
            if (paymentDto == null)
                throw new ArgumentNullException(nameof(paymentDto));

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return false;

            payment.Amount = paymentDto.Amount;
            payment.PaymentMethod = paymentDto.PaymentMethod;

            _context.Payments.Update(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return false;

            _context.Payments.Remove(payment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RefundPayment(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null || payment.Status != PaymentStatus.Completed)
                return false;

            if (string.IsNullOrEmpty(payment.TransactionId))
                return false;

            try
            {
                var service = new RefundService();
                var refund = await service.CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = payment.TransactionId
                });

                if (refund.Status == "succeeded")
                {
                    payment.Status = PaymentStatus.Refunded;
                    _context.Payments.Update(payment);

                    var transaction = new Models.Transaction
                    {
                        PaymentId = payment.Id,
                        UserId = payment.UserId,
                        Amount = payment.Amount,
                        TransactionType = "Refund",
                        TransactionDate = DateTime.UtcNow,
                        Status = "Completed",
                        Description = $"Refund for payment {payment.Id}"
                    };

                    _context.Transactions.Add(transaction);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (StripeException)
            {
                return false;
            }

            return false;
        }

        public async Task<PaymentStatus> CheckPaymentStatus(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            return payment?.Status ?? PaymentStatus.Failed;
        }

        public async Task<List<Models.Transaction>> ListTransactions(int userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}