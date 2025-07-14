using Doanwebcuoiki.Models;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext với SQL Server
builder.Services.AddDbContext<myContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("myconnection")));

// Đăng ký Session (CHỈ ĐƯỢC GỌI 1 LẦN, VÀ PHẢI TRƯỚC app.Build())
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Đổi về 2 tiếng, tuỳ ý bạn
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Dùng session (PHẢI ĐẶT SAU UseRouting, TRƯỚC Authorization)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
