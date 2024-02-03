using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebShop.Models.ViewModels;

public class ProductVM
{
    public Product Product { get; set; }
    public List<SelectListItem> CategoryList { get; set; }
}