using Microsoft.AspNetCore.Identity;

namespace WebShop.Models;

public class User : IdentityUser
{
    public string FullName { get; set; }
}