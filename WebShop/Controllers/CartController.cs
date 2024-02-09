using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Models;
using WebShop.Models.ViewModels;
using WebShop.Utilites;

namespace WebShop.Controllers;

[Authorize]
public class CartController : Controller
{
    private ApplicationDbContext _db;
    private readonly SessionServices _sessionManager;

    public CartController(ApplicationDbContext db, SessionServices sessionManager)
    {
        _db = db;
        _sessionManager = sessionManager;
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


    //  +1 в корзину
    [HttpPost]
    public IActionResult PlusToCart(int id)
    {
        var result = _sessionManager.PlusToCart(id);

        if (!result)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при добавлении товара в корзину";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
        }
        
        return RedirectToAction("Index",  new { id = id });
    }

    // -1 в корзину
    public IActionResult MinusToCart(int id)
    {
        var result = _sessionManager.MinusToCart(id);

        if (!result)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при удалении товара из корзины";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
        }
        
        return RedirectToAction("Index", new { id = id });
    }

    public IActionResult DeleteProduct(int id)
    {
        try
        {
            _sessionManager.RepairSession(id);

            var session = _sessionManager.GetSession(WebConst.SessionCart);

            var prodInSession = session.FirstOrDefault(x => x.ProductId == id);
            if (prodInSession != null)
            {
                session.Remove(prodInSession);
            }
        
            HttpContext.Session.Set(WebConst.SessionCart, session);
        
            return RedirectToAction("Index", new { id = id });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при удалении товара из корзины";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            return RedirectToAction("Index", new { id = id });
        }
    }
}