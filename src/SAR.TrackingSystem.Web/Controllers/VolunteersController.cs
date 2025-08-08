using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class VolunteersController : Controller
{
    private readonly ISarApiService _apiService;

    public VolunteersController(ISarApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index(int page = 1, string search = "")
    {
        ViewBag.SearchTerm = search;
        var volunteers = await _apiService.GetVolunteersAsync(page, 20, search);
        return View(volunteers);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Teams = await _apiService.GetTeamsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(VolunteerCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teams = await _apiService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            await _apiService.CreateVolunteerAsync(model);
            TempData["Success"] = "Ekip üyesi başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
            ViewBag.Teams = await _apiService.GetTeamsAsync();
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var volunteer = await _apiService.GetVolunteerByIdAsync(id);
        if (volunteer == null)
            return NotFound();

        ViewBag.Teams = await _apiService.GetTeamsAsync();
        
        var model = new VolunteerUpdateViewModel
        {
            TcKimlik = volunteer.TcKimlik,
            FullName = volunteer.FullName,
            TeamId = volunteer.TeamId,
            BloodType = volunteer.BloodType,
            Phone = volunteer.Phone,
            EmergencyContactName = volunteer.EmergencyContactName,
            EmergencyContactPhone = volunteer.EmergencyContactPhone,
            Buddy1 = volunteer.Buddy1,
            Buddy2 = volunteer.Buddy2
        };
        
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, VolunteerUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teams = await _apiService.GetTeamsAsync();
            return View(model);
        }

        try
        {
            var success = await _apiService.UpdateVolunteerAsync(id, model);
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

        ViewBag.Teams = await _apiService.GetTeamsAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _apiService.DeleteVolunteerAsync(id);
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
