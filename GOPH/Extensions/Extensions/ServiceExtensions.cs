using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.FileManager;
using GOPH.Services.CartServices;
using GOPH.Services.MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GOPH.Extensions.Extensions
{
    public static class ServiceExtensions
    {
        //LocalHost
        //Freeasphosting
        //smarter
        //https://www.msclusters.com/

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(
                    config.GetConnectionString("smarter"),
                    providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddIdentity<AppUser, IdentityRole>()
                          .AddEntityFrameworkStores<AppDbContext>()
                          .AddDefaultTokenProviders();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
           

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IFileServices, FileServices>();

            services.AddSingleton<ISendMailServices, SendMailServices>();

            services.AddTransient<ICartServices, CartServices>();


        }


        public static void ConfigureAuthorizationHandlerService(this IServiceCollection services)
        {

            services.AddAuthorization(options => {


                options.AddPolicy("Administrator", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator");

                });

                options.AddPolicy("Admin", builder => {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator", "Admin");

                });

                options.AddPolicy("Employee", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator", "Admin", "Employee");

                });

            });

          
        }


    }
}
