﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            try
            {
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
                return View(category);
            }
        }

        //  GET
        public IActionResult Edit(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return NotFound();
            }
    
            Models.Category obj = JsonSerializer.Deserialize<Models.Category>(category);
            if (obj == null) return NotFound();
            
            return View(obj);
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
            if (string.IsNullOrWhiteSpace(category))
            {
                return NotFound();
            }
    
            Models.Category obj = JsonSerializer.Deserialize<Models.Category>(category);
            if (obj == null) return NotFound();
            
            return View(obj);
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
                return View(category);
            }
        }
    }
}