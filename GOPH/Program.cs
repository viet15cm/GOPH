using GOPH.Extensions.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GOPH.DbContextLayer;
using GOPH.Entites;
using Microsoft.AspNetCore.Identity.UI.Services;
using GOPH.MailServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureMySqlContext(builder.Configuration);


// Add services to the container.
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();

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

app.UseRouting();

app.UseAuthorization();


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
