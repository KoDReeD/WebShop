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
                CategoryList = categories,
                PhotoStatus = PhotoStatus.None
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
            var oldPhotoName = productVm.Product.PhotoPath;
            string webRootPath = _webHostEnvironment.WebRootPath;
            //  абсолютынй путь до wwwroot + папака с картинками
            var uploads = webRootPath + WebConst.ImagePath;
            var statusPhoto = productVm.PhotoStatus;

            //  добавили файл
            if (statusPhoto == PhotoStatus.Add)
            {
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
            else
            {
                //  если NONE то просто выйдет на сохранение
                //  файл был, но сейчас файла нет, значит удаляем
                if (statusPhoto == PhotoStatus.Delete)
                {
                    var oldPath = uploads + oldPhotoName;

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    
                    productVm.Product.PhotoPath = null;
                }
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
            }
        }
        catch (Exception e)
        {
            if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
            return View(productVm);
        }

    }


    // GET
    public IActionResult Delete(string product)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(product)) return NotFound();
            
            var obj = JsonSerializer.Deserialize<Product>(product);
            return View(obj);
        }
        catch (Exception e)
        {
            return NotFound();
        }
        
    }

    // // Post
    [HttpPost]
    public IActionResult Delete(Product product)
    {
        try
        {
            var photoPath = product.PhotoPath;
            //  нужно удалить картинку
            if (!string.IsNullOrWhiteSpace(photoPath))
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                //  абсолютынй путь до wwwroot + папака с картинками
                var uploads = webRootPath + WebConst.ImagePath;

                if (System.IO.File.Exists(uploads + photoPath))
                {
                    System.IO.File.Delete(uploads + photoPath);
                }
            }
            
            _db.Product.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return View(product);
        }
    }
}