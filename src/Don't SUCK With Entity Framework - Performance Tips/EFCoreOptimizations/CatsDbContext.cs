using Microsoft.EntityFrameworkCore;

namespace EFCoreOptimizations
{
    public class CatsDbContext : DbContext
    {
        public DbSet<Cat> Cats { get; set; }

        public DbSet<Owner> Owners { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=.;Database=CatsDemoDb;Integrated Security=True;");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Cat>()
                .HasOne(c => c.Owner)
                .WithMany(o => o.Cats)
                .HasForeignKey(c => c.OwnerId);
        }
    }
}
