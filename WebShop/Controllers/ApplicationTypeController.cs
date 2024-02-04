using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            Models.ApplicationType obj;
            if (string.IsNullOrWhiteSpace(applicationType))
            {
                obj = new Models.ApplicationType();
            }
            else
            {
                obj = JsonSerializer.Deserialize<Models.ApplicationType>(applicationType);
            }
            return View(obj);
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public IActionResult AddEdit(Models.ApplicationType applicationType)
    {
        try
        {
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
        try
        {
            if (string.IsNullOrWhiteSpace(application))
            {
                return NotFound();
            }
            Models.ApplicationType obj = JsonSerializer.Deserialize<Models.ApplicationType>(application);

            return View(obj);
        }
        catch (Exception e)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Models.ApplicationType application)
    {
        try
        {
            var checkList = await _db.Product.Where(x => x.ApplicationTypeId == application.Id).ToListAsync();
            if (checkList.Count > 0) return RedirectToAction("Index");
            _db.ApplicationType.Remove(application);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            return View(application);
        }
    }
    

}