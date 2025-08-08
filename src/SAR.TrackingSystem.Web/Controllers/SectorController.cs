using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class SectorsController : Controller
{
    private readonly ISarApiService _apiService;

    public SectorsController(ISarApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var sectors = await _apiService.GetSectorsAsync();
        return View(sectors);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var sector = await _apiService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }
        return View(sector);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SectorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _apiService.CreateSectorAsync(model);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Sektör oluşturulurken bir hata oluştu.");
        return View(model);
    }

}