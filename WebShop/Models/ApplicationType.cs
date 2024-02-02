using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebShop.Models;

public class ApplicationType
{
    [Key] 
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}