using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services.Interfaces;

namespace SAR.TrackingSystem.Web.Controllers;

public class VolunteersController(
    IVolunteerService volunteerService,
    ITeamService teamService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        ViewBag.SearchTerm = search;
        var request = new PaginationRequest(page - 1, 10, search); // Convert to 0-based
        var volunteers = await volunteerService.GetVolunteersAsync(request);
        return View(volunteers);
    }

    public async Task<IActionResult> Create(string? qrId = null)
    {
        ViewBag.Teams = await teamService.GetTeamsAsync();
        
        var model = new VolunteerCreateViewModel();
        if (!string.IsNullOrEmpty(qrId))
        {
            model.QRId = qrId;
            ViewBag.PrefilledQRId = qrId;
        }
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(VolunteerCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teams = await teamService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            await volunteerService.CreateVolunteerAsync(model);
            TempData["Success"] = "Ekip üyesi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
            ViewBag.Teams = await teamService.GetTeamsAsync();
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var volunteer = await volunteerService.GetVolunteerByIdAsync(id);
        if (volunteer == null)
            return NotFound();

        ViewBag.Teams = await teamService.GetTeamsAsync();
        
        var model = new VolunteerUpdateViewModel
        {
            FullName = volunteer.FullName,
            TeamId = volunteer.TeamId,
            QRId = volunteer.QRId,
            Role = volunteer.Role
        };
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, VolunteerUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teams = await teamService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            var success = await volunteerService.UpdateVolunteerAsync(id, model);
            if (success)
            {
                TempData["Success"] = "Ekip üyesi başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Error = "Ekip üyesi bulunamadı.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
        }

        ViewBag.Teams = await teamService.GetTeamsAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await volunteerService.DeleteVolunteerAsync(id);
            if (success)
                TempData["Success"] = "Ekip üyesi başarıyla silindi.";
            else
                TempData["Error"] = "Ekip üyesi silinemedi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
