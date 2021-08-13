using Microsoft.EntityFrameworkCore;
using WebJobStarter.DTO;

namespace WebJobStarter.DbContext
{
    public class DBContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<Website> Website { get; set; }
        public DbSet<User> Users { get; set; }

        private DbContextOptions<DBContext> options;

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            this.options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasIndex(e => new {e.Hash}).IsUnique();
            modelBuilder.Entity<ProductType>().HasIndex(e => new {e.Type});
            modelBuilder.Entity<Website>().HasIndex(e => new {e.Name});
            modelBuilder.Entity<User>().HasIndex(e => new {e.password}).IsUnique();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:scrapperdbserver.database.windows.net,1433;Initial Catalog=ScrapperDk;Persist Security Info=False;User ID=Igor;Password=Moldova2014;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

    }
}