using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Context;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> op) : base(op)
    {
        
    }
    
    public DbSet<Category> Category { get; set; }
    public DbSet<ApplicationType> ApplicationType { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<User> User  { get; set; }
}