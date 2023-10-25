using GOPH.DbContextLayer;
using GOPH.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GOPH.Extensions.Extensions
{
    public static class ServiceExtensions
    {
        //LocalHost
        //Freeasphosting
        //msclusters
        //https://www.msclusters.com/

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(
                    config.GetConnectionString("LocalHost"),
                    providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddIdentity<AppUser, IdentityRole>()
                          .AddEntityFrameworkStores<AppDbContext>()
                          .AddDefaultTokenProviders();
        }



    }
}
