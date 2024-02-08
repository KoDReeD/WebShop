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
    private readonly SessionServices _sessionManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, SessionServices sessionManager)
    {
        _logger = logger;
        _db = db;
        _sessionManager = sessionManager;
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
            var shoppingCartList = _sessionManager.GetSession(WebConst.SessionCart);

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
        var result = _sessionManager.PlusToCart(id);

        if (!result)
        {
            TempData["ErrorMessage"] = "Произошла ошибка при добавлении товара в корзину";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
        }
        
        return RedirectToAction(nameof(Details), new { id = id });
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
        
        return RedirectToAction(nameof(Details), new { id = id });
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