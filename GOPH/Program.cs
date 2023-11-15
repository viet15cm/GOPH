using GOPH.Extensions.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg => {                    // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "GOPH";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 60, 0);    // Thời gian tồn tại của Session
});
// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureServiceManager();


builder.Services.ConfigureMySqlContext(builder.Configuration);

builder.Services.ConfigureApplicationCookie(option =>
{
    option.ExpireTimeSpan = TimeSpan.FromDays(1);
    option.LoginPath = "/Identity/Account/Login";
    option.LogoutPath = "/Identity/Account/Logout";
    option.AccessDeniedPath = $"/Identity/Account/Manager/AccessDenied";
});

builder.Services.ConfigureAuthorizationHandlerService();


builder.Services.AddSignalR();

builder.Services.AddControllers();

builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();   // Phục hồi thông tin đăng nhập (xác thực)
app.UseAuthorization();   // Phục hồi thông tinn về quyền của User



app.MapAreaControllerRoute(
            name: "Manager",
            pattern: "Manager/{controller}/{action}/{id?}",
            areaName: "Manager",
            defaults: new
            {
                controller = "Product",
                action = "index"
            }
        );

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
