using Microsoft.EntityFrameworkCore;
using WebShop.Context;

var builder = WebApplication.CreateBuilder(args);

//  добавление контекста БД
// builder.Services.AddDbContext<ApplicationDbContext>(op => op.UseNpgsql(
//     builder.Configuration.GetConnectionString("PostgreConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(op => op.UseNpgsql(
    builder.Configuration.GetConnectionString("LocalPostgreConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(op =>
    {
        op.IdleTimeout = TimeSpan.FromMinutes(10);
        op.Cookie.HttpOnly = true;
        op.Cookie.IsEssential = true;
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();