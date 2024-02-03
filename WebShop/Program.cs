using Microsoft.EntityFrameworkCore;
using WebShop.Context;

var builder = WebApplication.CreateBuilder(args);

//  добавление контекста БД
// builder.Services.AddDbContext<ApplicationDbContext>(op => op.UseNpgsql(
//     builder.Configuration.GetConnectionString("PostgreConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(op => op.UseNpgsql(
    builder.Configuration.GetConnectionString("LocalPostgreConnection")));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();