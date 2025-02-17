using System;

namespace PaymentService.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public Transaction()
        {
            TransactionType = string.Empty;
            Status = "Pending";
            Description = string.Empty;
            CreatedDate = DateTime.UtcNow;
        }
    }
}