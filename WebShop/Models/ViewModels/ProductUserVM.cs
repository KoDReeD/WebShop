namespace WebShop.Models.ViewModels;

public class ProductUserVM
{
    public User User { get; set; }
    public List<Product> ProductList { get; set; }

    public ProductUserVM()
    {
        ProductList = new List<Product>();
    }
}