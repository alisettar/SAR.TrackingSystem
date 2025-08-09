using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class TeamsController : Controller
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _teamService.GetTeamsAsync();
        return View(teams);
    }

    public async Task<IActionResult> Details(Guid id, int page = 1)
    {
        var team = await _teamService.GetTeamByIdAsync(id);
        if (team == null)
        {
            return NotFound();
        }
        
        var request = new PaginationRequest(page - 1, 10); // Convert to 0-based
        var members = await _teamService.GetTeamMembersAsync(id, request);
        
        var model = new TeamDetailsViewModel
        {
            Id = team.Id,
            Name = team.Name,
            Code = team.Code,
            Members = [.. members.Items]
        };
        
        ViewBag.MembersPagination = members;
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _teamService.CreateTeamAsync(model);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Ekip oluşturulurken bir hata oluştu.");
        return View(model);
    }

}