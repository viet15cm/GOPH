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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            builder.Entity<Commodity>(entity => {
                entity.HasIndex(p => p.Id).IsUnique();
            });

            builder.Entity<CommodityGroup>(entity => {
                entity.HasIndex(p => p.Id).IsUnique();
            });

            builder.Entity<Product>(entity => {
                entity.HasIndex(p => p.Id).IsUnique();
            });


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
