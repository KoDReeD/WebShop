using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebShop.Models;
using WebShop.Utilites;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebShop.Context
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _db.Category.ToListAsync();
            return View(list);
        }

        //  GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // токен защиты, проверка что действителен в методе
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                var name = category.Title?.ToLower() ?? "";
                var checkName = await _db.Category.FirstOrDefaultAsync(x => x.Title.ToLower() == name);
            
                if (checkName != null)
                {
                    TempData["ErrorMessage"] = "Category with that name already exists";
                    TempData["ErrorType"] = SweetAlertType.error.ToString();
                    return View(category);
                }
                
                if (ModelState.IsValid)
                {
                    _db.Category.Add(category);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
                TempData["ErrorType"] = SweetAlertType.error.ToString();
                
                return View(category);
            }
        }

        //  GET
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _db.Category.FindAsync(id);
                if (category == null) return NotFound();

                return View(category);
            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
    
            
        }

        [HttpPost]
        // токен защиты, проверка что действителен в методе
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                var name = category.Title?.ToLower() ?? "";
                var checkName = await _db.Category.AsNoTracking().FirstOrDefaultAsync(x => x.Title.ToLower() == name);
            
                if (checkName != null && checkName.Id != category.Id)
                {
                    TempData["ErrorMessage"] = "Category with that name already exists";
                    TempData["ErrorType"] = SweetAlertType.error.ToString();
                    return View(category);
                }
                
                _db.Category.Update(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
                TempData["ErrorType"] = SweetAlertType.error.ToString();
                
                return View(category);
            }
        }


        //  GET
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0) return NotFound();
                
                var category = await _db.Category.FindAsync(id);
                if (category == null) return NotFound();

                return View(category);
            }
            catch (Exception e)
            {
                return View("Index");
            }
        }
        
        

        [HttpPost]
        public async Task<IActionResult> Delete(Category category)
        {
            try
            {
                var check = await _db.Product.Where(x => x.CategoryId == category.Id).ToListAsync();
                if (check.Count > 0)
                {
                    return RedirectToAction("Index");
                }
                _db.Category.Remove(category);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
                TempData["ErrorType"] = SweetAlertType.error.ToString();

                return View(category);
            }
        }
    }
}