using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Models;
using WebShop.Models.ViewModels;
using WebShop.Utilites;

namespace WebShop.Controllers;

public class CartController : Controller
{
    private ApplicationDbContext _db;

    public CartController(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            List<ShoppingCart> cartList = new List<ShoppingCart>();
            var session = HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);

            if (session != null && session.Count > 0)
            {
                cartList = session;
            } 
            //  из продукт беру только те id которые содержаться в карзине
            var productIds = cartList.Select(i => i.ProductId).ToList();

            var products = await _db.Product
                .Where(x => productIds.Contains(x.Id))
                .ToListAsync();

            List<CartItem> items = new List<CartItem>();
            foreach (var prod in products)
            {
                var cart = cartList.FirstOrDefault(x => x.ProductId == prod.Id);
                items.Add(new CartItem(){Product = prod, Amount = cart.Amount});
            }

            return View(items);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index", "Product");
        }
    }
}