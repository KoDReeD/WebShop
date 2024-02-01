using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebShop.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebShop.Context
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var list = _db.Category.ToList();
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
        public IActionResult Create(Category category)
        {
            _db.Category.Add(category);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //  GET
        public IActionResult Edit(string category)
        {
            Category obj;
            try
            {
                obj = JsonSerializer.Deserialize<Category>(category);
                return View(obj);
            }
            catch (Exception e)
            {
                obj = new Category();
                return View(obj);
            }
        }

        [HttpPost]
        // токен защиты, проверка что действителен в методе
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            try
            {
                _db.Category.Update(category);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(category);
            }
        }

        
        //  GET
        public IActionResult Delete(string category)
        {
            Category obj;

            try
            {
                obj = JsonSerializer.Deserialize<Category>(category);
                return View(obj);
            }
            catch (Exception e)
            {
                obj = new Category();
                return View(obj);
            }
        }

        [HttpPost]
        public IActionResult Delete(Category category)
        {
            try
            {
                _db.Category.Remove(category);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(category);
            }
        }
    }
}
