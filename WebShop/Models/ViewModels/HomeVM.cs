namespace WebShop.Models.ViewModels;

public class HomeVM
{
    public IList<Product> Products { get; set; }
    public IList<Category> Categories { get; set; }
}