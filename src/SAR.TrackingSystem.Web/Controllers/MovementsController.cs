using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class MovementsController : Controller
{
    private readonly ISarApiService _apiService;

    public MovementsController(ISarApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var movements = await _apiService.GetMovementsAsync(page, 20);
        return View(movements);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Volunteers = await _apiService.GetVolunteersAsync(1, 1000);
        ViewBag.Sectors = await _apiService.GetSectorsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MovementCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Volunteers = await _apiService.GetVolunteersAsync(1, 1000);
            ViewBag.Sectors = await _apiService.GetSectorsAsync();
            return View(model);
        }

        try
        {
            await _apiService.CreateMovementAsync(model);
            TempData["Success"] = "Hareket başarıyla kaydedildi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
            ViewBag.Volunteers = await _apiService.GetVolunteersAsync(1, 1000);
            ViewBag.Sectors = await _apiService.GetSectorsAsync();
            return View(model);
        }
    }
}
