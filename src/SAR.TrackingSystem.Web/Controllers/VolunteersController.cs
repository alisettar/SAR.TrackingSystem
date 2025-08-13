using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services;
using SAR.TrackingSystem.Web.Services.Interfaces;

namespace SAR.TrackingSystem.Web.Controllers;

public class VolunteersController(
    IVolunteerService volunteerService,
    ITeamService teamService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string search = "", string? filter = null)
    {
        ViewBag.SearchTerm = search;
        ViewBag.Filter = filter;
        
        // Filter title for display
        ViewBag.FilterTitle = filter switch
        {
            "inHub" => "BoO'da Bulunan Ekip Üyeleri",
            "inSector" => "Sektörde Bulunan Ekip Üyeleri", 
            "exited" => "Çıkış Yapan Ekip Üyeleri",
            "notEntered" => "Katılmayan Ekip Üyeleri",
            _ => "Tüm Ekip Üyeleri"
        };
        
        var request = new PaginationRequest(page - 1, 10, search); // Convert to 0-based
        var volunteers = await volunteerService.GetVolunteersAsync(request, filter);
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
    
    [HttpGet]
    public async Task<IActionResult> Timeline(Guid id)
    {
        try
        {
            var movements = await volunteerService.GetVolunteerMovementHistoryAsync(id);
            return Json(movements);
        }
        catch
        {
            return BadRequest();
        }
    }

    public async Task<IActionResult> BooSectorTeams()
    {
        var teamIds = await volunteerService.GetTeamIdsWithNonArrivedVolunteersAsync();

        var teams = new List<TeamViewModel>();
        foreach (var teamId in teamIds)
        {
            var team = await teamService.GetTeamByIdAsync(teamId);
            if (team != null)
            {
                teams.Add(team);
            }
        }

        return View(teams);
    }

    public async Task<IActionResult> BooSectorTeamsDetails(Guid teamId, int page = 1)
    {
        var team = await teamService.GetTeamByIdAsync(teamId);
        if (team == null)
        {
            return NotFound();
        }

        var request = new PaginationRequest(page - 1, 10); // Convert to 0-based
        var members = await teamService.GetTeamMembersAsync(teamId, request);

        var model = new TeamDetailsViewModel
        {
            Id = team.Id,
            Name = team.Name,
            Code = team.Code,
            City = team.City,
            Members = [.. members.Items],
            TotalCount = members.TotalCount,
        };

        ViewBag.MembersPagination = members;
        return View(model);
    }
}
