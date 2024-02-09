using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Utilites;

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


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders().AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddMvc();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<SessionServices>();
builder.Services.AddTransient<IEmailSender, PochtaSender>();

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// app.MapRazorPages();
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapControllerRoute(
//         name: "default",
//         pattern: "{controller=Home}/{action=Index}/{id?}");
//     endpoints.MapRazorPages();
// });

app.Run();
