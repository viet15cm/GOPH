using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.FileManager;
using GOPH.Security.Requirements;
using GOPH.Services.CallApiServices;
using GOPH.Services.CartServices;
using GOPH.Services.MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GOPH.Extensions
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
                    config.GetConnectionString("LocalHost"),
                    providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddIdentity<AppUser, IdentityRole>()
                          .AddEntityFrameworkStores<AppDbContext>()
                          .AddDefaultTokenProviders();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {

            services.AddHttpClient<IHttpClientServiceImplementation, HttpClientStreamService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IFileServices, FileServices>();

            services.AddSingleton<ISendMailServices, SendMailServices>();

            services.AddTransient<ICartServices, CartServices>();
            services.AddTransient<IFileServices, FileServices>();


            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 8; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;  // Email là duy nhất

                // Cấu hình đăng nhập.

                options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

            });

        }


        public static void ConfigureAuthorizationHandlerService(this IServiceCollection services)
        {

            services.AddAuthorization(options =>
            {


                options.AddPolicy("Administrator", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator");

                });

                options.AddPolicy("Admin", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator", "Admin");

                });

                options.AddPolicy("Employee", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator", "Admin", "Employee");

                });

                options.AddPolicy("Customer", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("Administrator", "Admin", "Employee", "Customer");

                });

            });


            services.AddTransient<IAuthorizationHandler, CanOptionWholesaleHandler>();


        }


    }
}
