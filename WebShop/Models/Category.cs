using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [DisplayName("Name")]
    public string Title { get; set; }
    [DisplayName("Display Order")]
    public int DisplayOrder { get; set; }
}