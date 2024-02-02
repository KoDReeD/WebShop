using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [DisplayName("Name")]
    [Required]
    public string Title { get; set; }
    [DisplayName("Display Order")]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Display Order field must be greater than 0")]
    public int DisplayOrder { get; set; }
}