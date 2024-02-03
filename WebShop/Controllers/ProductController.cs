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
    private static ApplicationDbContext _db;
    private IWebHostEnvironment _webHostEnvironment;

    public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var products = await _db.Product
            .Include(x => x.Category)
            .ToListAsync();
        return View(products);
    }

    // GET
    public async Task<IActionResult> AddEdit(string product)
    {
        try
        {
            var categories = await GetCategoryList();
            if (categories == null) return View("Index");

            ProductVM objVM = new ProductVM()
            {
                Product = string.IsNullOrWhiteSpace(product)
                    ? new Product()
                    : JsonSerializer.Deserialize<Product>(product),
                CategoryList = categories
            };

            return View(objVM);
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }

    private static async Task<List<SelectListItem>> GetCategoryList()
    {
        try
        {
            var list = await _db.Category.Select(x => new SelectListItem()
            {
                Text = x.Title,
                Value = x.Id.ToString()
            }).ToListAsync();
            return list;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    // Post
    [HttpPost]
    public async Task<IActionResult> AddEdit(ProductVM productVm)
    {
        var files = HttpContext.Request.Form.Files;
        try
        {
            //  выбран ли файл
            if (files != null)
            {
                var oldPhotoName = productVm.Product.PhotoPath;
                string webRootPath = _webHostEnvironment.WebRootPath;
                var uploads = webRootPath + WebConst.ImagePath;
                
                //  если было фото удаляем
                if (!string.IsNullOrWhiteSpace(oldPhotoName))
                {
                    var oldPath = uploads + oldPhotoName;

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                
                var extension = Path.GetExtension(files[0].FileName);
                var file = files[0];
                
                var fileName = Guid.NewGuid() + extension;
                var fullPath = Path.Combine(uploads, fileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                productVm.Product.PhotoPath = fileName;
            }
        }
        catch (Exception e)
        {
            if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
            return View(productVm);
        }

        //  CREATE
        if (productVm.Product.Id == 0)
        {
            try
            {
                _db.Product.Add(productVm.Product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
                return View(productVm);
            }
        }
        //  EDIT
        else
        {
            try
            {
                _db.Product.Update(productVm.Product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
                return View(productVm);
            }
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }


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