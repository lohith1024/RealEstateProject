using Microsoft.EntityFrameworkCore;
using PropertyManagementService.Models;

namespace PropertyManagementService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId);

            // Indexes
            modelBuilder.Entity<Property>()
                .HasIndex(p => p.OwnerId);

            modelBuilder.Entity<Property>()
                .HasIndex(p => new { p.City, p.State });

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Type);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Status);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Price);

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.PropertyId);

            modelBuilder.Entity<Review>()
                .HasIndex(r => r.UserId);

            modelBuilder.Entity<PropertyImage>()
                .HasIndex(pi => pi.PropertyId);
        }
    }
}