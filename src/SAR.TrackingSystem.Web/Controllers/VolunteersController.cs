using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class VolunteersController : Controller
{
    private readonly IVolunteerService _volunteerService;
    private readonly ITeamService _teamService;

    public VolunteersController(IVolunteerService volunteerService, ITeamService teamService)
    {
        _volunteerService = volunteerService;
        _teamService = teamService;
    }

    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        ViewBag.SearchTerm = search;
        var volunteers = await _volunteerService.GetVolunteersAsync(page, 20, search);
        return View(volunteers);
    }

    public async Task<IActionResult> Create(string? qrId = null)
    {
        ViewBag.Teams = await _teamService.GetTeamsAsync();
        
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
            ViewBag.Teams = await _teamService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            await _volunteerService.CreateVolunteerAsync(model);
            TempData["Success"] = "Ekip üyesi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
            ViewBag.Teams = await _teamService.GetTeamsAsync();
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var volunteer = await _volunteerService.GetVolunteerByIdAsync(id);
        if (volunteer == null)
            return NotFound();

        ViewBag.Teams = await _teamService.GetTeamsAsync();
        
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
            ViewBag.Teams = await _teamService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            var success = await _volunteerService.UpdateVolunteerAsync(id, model);
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

        ViewBag.Teams = await _teamService.GetTeamsAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _volunteerService.DeleteVolunteerAsync(id);
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
