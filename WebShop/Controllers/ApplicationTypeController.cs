﻿using Microsoft.AspNetCore.Mvc;
using WebShop.Context;
using WebShop.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebShop.Controllers;

public class ApplicationType : Controller
{
    private ApplicationDbContext _db;

    public ApplicationType(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET
    public IActionResult Index()
    {
        var list = _db.ApplicationType.ToList();
        return View(list);
    }

    // [Route("AddEdit/{applicationType}")]
    public IActionResult AddEdit(string applicationType)
    {
        try
        {
            Models.ApplicationType obj = JsonSerializer.Deserialize<Models.ApplicationType>(applicationType);
            if (obj == null) obj = new Models.ApplicationType();
            return View(obj);
        }
        catch (Exception e)
        {
            return View(new Models.ApplicationType());
        }
    }

    [HttpPost]
    public IActionResult AddEdit(Models.ApplicationType applicationType)
    {
        try
        {
            throw new Exception();
            //  добавление
            if (applicationType.Id == 0)
            {
                _db.ApplicationType.Add(applicationType);
            }
            //  редактирование
            else
            {
                _db.ApplicationType.Update(applicationType);
            }

            _db.SaveChanges();
            
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return View("AddEdit", applicationType);
        }
        
        
    }

    //  GET
    public IActionResult Delete(string application)
    {
        Models.ApplicationType obj;

        try
        {
            obj = JsonSerializer.Deserialize<Models.ApplicationType>(application);
            return View(obj);
        }
        catch (Exception e)
        {
            obj = new Models.ApplicationType();
            return View(obj);
        }
    }

    [HttpPost]
    public IActionResult Delete(Models.ApplicationType application)
    {
        try
        {
            _db.ApplicationType.Remove(application);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return View(application);
        }
    }
    

}