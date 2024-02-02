using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double Cost { get; set; }
    
    [Range(0, 99)]
    public int Discount { get; set; }
    
    public string? PhotoPath { get; set; }
    
    [Display(Name = "Category Type")]
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }
}