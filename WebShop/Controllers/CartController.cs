using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}