using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Models;
using WebShop.Models.ViewModels;

namespace WebShop.Controllers;

public class ProductController : Controller
{
    private ApplicationDbContext _db;

    public ProductController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET
    public IActionResult Index()
    {
        var products = _db.Product
            .Include(x => x.Category)
            .ToList();
        return View(products);
    }

    // GET
    public IActionResult AddEdit(string product)
    {
        try
        {
            ProductVM objVM = new ProductVM()
            {
                Product = string.IsNullOrWhiteSpace(product) ? new Product() : JsonSerializer.Deserialize<Product>(product),
                CategoryList = _db.Category.Select(x => new SelectListItem()
                {
                    Text = x.Title,
                    Value = x.Id.ToString()
                })
            };
            
            return View(objVM);
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }

    // // Post
    // [HttpPost]
    // public IActionResult AddEdit()
    // {
    // }
    //
    // GET
    public IActionResult Delete(string product)
    {
        return View();
    }
    
    // // Post
    // [HttpPost]
    // public IActionResult Delete()
    // {
    // }
}