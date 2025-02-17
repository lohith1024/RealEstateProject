using Microsoft.EntityFrameworkCore;
using PaymentService.Models;

namespace PaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.BookingId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.UserId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.PaymentId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.TransactionDate);
        }
    }
}