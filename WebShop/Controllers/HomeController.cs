using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Models;
using WebShop.Models.ViewModels;
using WebShop.Utilites;

namespace WebShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            HomeVM homeVm = new HomeVM()
            {
                Products = await _db.Product
                    .Include(x => x.Category)
                    .Include(x => x.ApplicationType)
                    .ToListAsync(),
                Categories = await _db.Category.ToListAsync(),
            };
            return View(homeVm);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
        }
    }

    //  метод чинит сессию
    private void RepairSession(int productId)
    {
        try
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var session = HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);
            if (session != null && session.Any())
            {
                shoppingCartList = session;
            }

            var currentProduct = shoppingCartList.SingleOrDefault(x => x.ProductId == productId);

            //  Добавляем первый раз в корзину или ошибочно добавлено 2 предмета
            if (currentProduct == null)
            {
                //  проверОчка вдруг 2 объекта с одинаковым id
                var list = shoppingCartList.Where(x => x.ProductId == productId);
                if (!list.Any()) shoppingCartList.Add(new ShoppingCart() { ProductId = productId, Amount = 1 });
                else
                {
                    int amount = 0;
                    // объединить количество и удалить
                    foreach (var item in list)
                    {
                        amount = item.Amount;
                        shoppingCartList.Remove(item);
                    }

                    shoppingCartList.Add(new ShoppingCart() { ProductId = productId, Amount = amount });
                }
            }

            HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);
        }
        catch (Exception e)
        {
        }
    }

    private List<ShoppingCart> GetSession(string key)
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        var session = HttpContext.Session.Get<List<ShoppingCart>>(key);
        if (session != null && session.Count() > 0)
        {
            shoppingCartList = session;
        }

        return shoppingCartList;
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var shoppingCartList = GetSession(WebConst.SessionCart);

            var prod = await _db.Product
                .Include(x => x.Category)
                .Include(x => x.ApplicationType).FirstOrDefaultAsync(x => x.Id == id);

            if (prod == null) return NotFound();


            var detailsVM = new DetailsProductVM()
            {
                Product = prod,
                ExistsInCard = shoppingCartList.FirstOrDefault(x => x.ProductId == id) == null
                    ? false
                    : true,
                Count = shoppingCartList.FirstOrDefault(x => x.ProductId == id)?.Amount ?? 0
            };


            return View(detailsVM);
        }
        catch (Exception e)
        {
            return View("Index");
        }
    }

    //  +1 в корзину
    [HttpPost]
    public IActionResult PlusToCart(int id)
    {
        try
        {
            var shoppingCartList = GetSession(WebConst.SessionCart);
            
            //  починили сессию
            RepairSession(id);

            var currentProduct = shoppingCartList.FirstOrDefault(x => x.ProductId == id);

            //  Добавляем первый раз в корзину
            if (currentProduct == null)
            {
                shoppingCartList.Add(new ShoppingCart() { ProductId = id, Amount = 1 });
            }
            //  есть в корзине
            else
            {
                shoppingCartList.Remove(currentProduct);
                currentProduct.Amount = ++currentProduct.Amount;
                shoppingCartList.Add(currentProduct);
            }

            //  храним серелизованный list в сессии
            HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при добавлении в корзину";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            return RedirectToAction("Details", new { id = id });
        }
    }

    // -1 в корзину
    public IActionResult MinusToCart(int id)
    {
        try
        {
            var shoppingCartList = GetSession(WebConst.SessionCart);

            var currentProduct = shoppingCartList.FirstOrDefault(x => x.ProductId == id);
            var Productamount = currentProduct.Amount;
            //  уменьшаем количество
            if (currentProduct != null && Productamount >= 1)
            {
                shoppingCartList.Remove(currentProduct);
                --Productamount;
                
                if (Productamount > 0)
                {
                    currentProduct.Amount = Productamount;
                    shoppingCartList.Add(currentProduct);
                }
            }

            HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Details), new { id = id });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при добавлении в корзину";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            return RedirectToAction("Details", new { id = id });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}