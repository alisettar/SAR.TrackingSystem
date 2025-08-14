using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services.Interfaces;

namespace SAR.TrackingSystem.Web.Controllers;

public class SectorsController(
    ISectorService sectorService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var sectors = await sectorService.GetSectorsAsync();
        
        // Backdoor for delete functionality
        ViewBag.EnableDelete = Request.Query.ContainsKey("admin") && Request.Query["admin"] == "true";
        
        return View(sectors);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var sector = await sectorService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }
        return View(sector);
    }

    [HttpGet]
    [Route("sectors/{id:guid}/statistics")]
    public async Task<IActionResult> Statistics(Guid id)
    {
        var statistics = await sectorService.GetSectorStatisticsAsync(id);
        if (statistics == null)
        {
            return NotFound();
        }
        return Json(statistics);
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

        var result = await sectorService.CreateSectorAsync(model);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Sektör oluşturulurken bir hata oluştu.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sector = await sectorService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }

        // Kritik sektörleri silme işlemini engelle
        if (IsCriticalSector(sector.Code))
        {
            TempData["Error"] = "Bu sektör sistem için kritik olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

        return View(sector);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var sector = await sectorService.GetSectorByIdAsync(id);
        if (sector == null)
        {
            return NotFound();
        }

        // Kritik sektörleri silme işlemini engelle
        if (IsCriticalSector(sector.Code))
        {
            TempData["Error"] = "Bu sektör sistem için kritik olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

        var result = await sectorService.DeleteSectorAsync(id);
        if (result)
        {
            TempData["Success"] = "Sektör başarıyla silindi.";
        }
        else
        {
            TempData["Error"] = "Sektör silinirken bir hata oluştu.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static bool IsCriticalSector(string code)
    {
        var criticalSectors = new[] { "Entry", "BoO", "Exit" };
        return criticalSectors.Contains(code, StringComparer.OrdinalIgnoreCase);
    }

    [HttpPut("/api/sectors/{id}/counts")]
    public async Task<IActionResult> UpdateCounts(Guid id, [FromBody] UpdateCountsRequest request)
    {
        try
        {
            var result = await sectorService.UpdateSectorCountsAsync(id, request.RescuedCount, request.ExtricatedCount);
            return result ? Ok() : NotFound();
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            return BadRequest(new { error = "Geçersiz değerler girdiniz." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Bir hata oluştu." });
        }
    }

    public class UpdateCountsRequest
    {
        public int RescuedCount { get; set; }
        public int ExtricatedCount { get; set; }
    }

}