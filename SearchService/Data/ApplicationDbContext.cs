using Microsoft.EntityFrameworkCore;
using SearchService.Models;

namespace SearchService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Location);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Price);

            modelBuilder.Entity<Property>()
                .HasIndex(p => p.Type);
        }
    }
}