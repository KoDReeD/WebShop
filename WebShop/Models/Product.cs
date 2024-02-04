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
    [Range(1, double.MaxValue, ErrorMessage = "Cost field must be from 0 to 99")]
    public double Cost { get; set; }
    
    [Range(0, 99, ErrorMessage = "Cost field must be greater than 0 and less than 100")]
    public int Discount { get; set; }
    
    public string? PhotoPath { get; set; }
    
    [Display(Name = "Category Type")]
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    [Required]
    public virtual Category Category { get; set; }

    [Display(Name = "Application Type")]
    public int ApplicationTypeId { get; set; }

    [ForeignKey("ApplicationTypeId")]
    [Required]
    public ApplicationType ApplicationType { get; set; }
}