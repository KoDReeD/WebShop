namespace WebShop.Models.ViewModels;

public class DetailsProductVM
{
    public DetailsProductVM()
    {
        Product = new Product();
    }
    public Product Product { get; set; }
    public bool ExistsInCard { get; set; }
    
    public int Count { get; set; }
}