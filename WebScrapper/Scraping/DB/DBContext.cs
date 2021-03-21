using Microsoft.EntityFrameworkCore;
using WebScrapper.Scraping.DTO;

namespace WebScrapper.Scraping.ScrappingFluggerDk.DB
{
    public class DBContext: DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<Website> Website { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = Flugger.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasIndex(e => new {e.Hash}).IsUnique();
            modelBuilder.Entity<ProductType>().HasIndex(e => new {e.Type});
            modelBuilder.Entity<Website>().HasIndex(e => new {e.Name});
            modelBuilder.Entity<User>().HasIndex(e => new {e.password}).IsUnique();
        }
    }
}