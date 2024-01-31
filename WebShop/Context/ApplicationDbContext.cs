using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> op) : base(op)
    {
        
    }
    
    public DbSet<Category> Category { get; set; }
    public DbSet<ApplicationType> ApplicationType { get; set; }
}