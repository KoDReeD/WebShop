using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Context;
using WebShop.Models;
using WebShop.Models.ViewModels;
using WebShop.Utilites;

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
            .Include(x => x.ApplicationType)
            .ToListAsync();
        return View(products);
    }

    // GET
    public async Task<IActionResult> AddEdit(int id)
    {
        try
        {
            var categories = await GetCategoryList();
            var applications = await GetApplicationList();

            if (categories == null) return View("Index");
            if (applications == null) return View("Index");

            ProductVM objVM = new ProductVM()
            {
                ApplicationTypeList = applications,
                CategoryList = categories,
                PhotoStatus = PhotoStatus.None,
            };

            if (id == 0)
            {
                objVM.Product = new Product();
            }
            else
            {
                var product = await _db.Product.FirstOrDefaultAsync(x => x.Id == id);
                if (product == null) return NotFound();

                objVM.Product = product;
            }

            return View(objVM);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
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

    private static async Task<List<SelectListItem>> GetApplicationList()
    {
        try
        {
            var list = await _db.ApplicationType.Select(x => new SelectListItem()
            {
                Text = x.Name,
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
                var photoPath = uploads + oldPhotoName;
                //  добавили вместо существующего, то удаляем старое
                if (!string.IsNullOrWhiteSpace(oldPhotoName))
                {
                    if (System.IO.File.Exists(photoPath))
                    {
                        System.IO.File.Delete(photoPath);
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
            //  удаление
            else if (statusPhoto == PhotoStatus.Delete)
            {
                var oldPath = uploads + oldPhotoName;

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                productVm.Product.PhotoPath = null;
            }
        }
        catch (Exception e)
        {
            
            if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
            if (productVm.ApplicationTypeList == null) productVm.ApplicationTypeList = await GetApplicationList();

            TempData["ErrorMessage"] = "Произошла ошибка при сохранении изображения";
            TempData["ErrorType"] = SweetAlertType.error.ToString();

            return View(productVm);
        }

        try
        {
            throw new Exception();
            //  CREATE
            if (productVm.Product.Id == 0)
            {
                _db.Product.Add(productVm.Product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            //  EDIT
            else
            {
                _db.Product.Update(productVm.Product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        catch (Exception e)
        {
            if (productVm.CategoryList == null) productVm.CategoryList = await GetCategoryList();
            if (productVm.ApplicationTypeList == null) productVm.ApplicationTypeList = await GetApplicationList();

            TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
            TempData["ErrorType"] = SweetAlertType.error.ToString();

            return View(productVm);
        }
    }


    // GET
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id == 0) return NotFound();

            var product = await _db.Product
                .Include(x => x.ApplicationType)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
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
            TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
            TempData["ErrorType"] = SweetAlertType.error.ToString();

            return View(product);
        }
    }
}