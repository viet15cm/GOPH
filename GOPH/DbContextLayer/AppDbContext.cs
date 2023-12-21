
using GOPH.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace GOPH.DbContextLayer
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        protected AppDbContext()
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Commodity> Commodities { get; set; }

        public DbSet<CommodityGroup> CommodityGroups { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<GOPH.Entites.Image> Images { get; set; }

        public DbSet<Wholesale> Wholesales { get; set; }

        public DbSet<Event> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            builder.Entity<Commodity>(entity =>
            {
                entity.HasIndex(p => p.Id).IsUnique();
            });

            builder.Entity<CommodityGroup>(entity =>
            {
                entity.HasIndex(p => p.Id).IsUnique();
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Id).IsUnique();
            });


            builder.Entity<OrderProduct>()
           .HasKey(bc => new { bc.OderId, bc.ProductId });
            builder.Entity<OrderProduct>()
                .HasOne(bc => bc.Product)
                .WithMany(b => b.OrderProducts)
                .HasForeignKey(bc => bc.ProductId);
            builder.Entity<OrderProduct>()
                .HasOne(bc => bc.Order)
                .WithMany(c => c.OrderProducts)
                .HasForeignKey(bc => bc.OderId);


            foreach (var item in builder.Model.GetEntityTypes())
            {
                var tableName = item.GetTableName();

                if (tableName.StartsWith("AspNet"))
                {
                    item.SetTableName(tableName.Substring(6));
                }

            }

        }

    }
}
