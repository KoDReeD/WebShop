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

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var session = HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);
            if (session != null && session.Count() > 0)
            {
                shoppingCartList = session;
            }

            var prod = await _db.Product
                .Include(x => x.Category)
                .Include(x => x.ApplicationType).FirstOrDefaultAsync(x => x.Id == id);

            if (prod == null) return NotFound();

            var detailsVM = new DetailsProductVM()
            {
                Product = prod,
                ExistsInCard = shoppingCartList.FirstOrDefault(x => x.ProductId == id) == null
                    ? false : true
            };


            return View(detailsVM);
        }
        catch (Exception e)
        {
            return View("Index");
        }
    }

    //  Добавление в карзину
    [HttpPost]
    public IActionResult DetailsPost(int id)
    {
        try
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var session = HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);
            if (session != null && session.Count() > 0)
            {
                shoppingCartList = session;
            }

            //  храним серелизованный list в сессии
            shoppingCartList.Add(new ShoppingCart() { ProductId = id });
            HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
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

    public IActionResult RemoveFromCart(int id)
    {
        try
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            var session = HttpContext.Session.Get<List<ShoppingCart>>(WebConst.SessionCart);
            if (session != null && session.Count() > 0)
            {
                shoppingCartList = session;
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(x => x.ProductId == id);
            if (itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WebConst.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при добавлении в корзину";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            return RedirectToAction("Details", new { id = id });
        }
    }
}