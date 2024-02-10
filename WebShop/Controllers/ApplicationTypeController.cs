using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebShop.Context;
using WebShop.Models;
using WebShop.Utilites;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebShop.Controllers;

[Authorize(Roles = "Admin")]
public class ApplicationType : Controller
{
    private ApplicationDbContext _db;

    public ApplicationType(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var list = await _db.ApplicationType.ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> AddEdit(int id)
    {
        try
        {
            if (id == 0)
            {
                return View(new Models.ApplicationType());
            }
            else
            {
                var application = await _db.ApplicationType.FindAsync(id);
                if (application == null) return NotFound();

                return View(application);
            }
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEdit(Models.ApplicationType applicationType)
    {
        try
        {
            var name = applicationType.Name?.ToLower() ?? "";
            var checkName = await _db.ApplicationType.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == name);
            
            if (checkName != null && checkName.Id != applicationType.Id)
            {
                TempData["ErrorMessage"] = "Application Type with that name already exists";
                TempData["ErrorType"] = SweetAlertType.error.ToString();
                return View("AddEdit", applicationType);
            }

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
            TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            return View("AddEdit", applicationType);
        }
    }

    //  GET
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            if (id == 0) return NotFound();

            var application = await _db.ApplicationType.FindAsync(id);
            if (application == null) return NotFound();

            return View(application);
        }
        catch (Exception e)
        {
            return RedirectToAction("Index");
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
            TempData["ErrorMessage"] = "Произошла ошибка при сохранении";
            TempData["ErrorType"] = SweetAlertType.error.ToString();
            
            return View(application);
        }
    }
}