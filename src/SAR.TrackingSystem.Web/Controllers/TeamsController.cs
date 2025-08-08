using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class TeamsController : Controller
{
    private readonly ISarApiService _apiService;

    public TeamsController(ISarApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _apiService.GetTeamsAsync();
        return View(teams);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var team = await _apiService.GetTeamByIdAsync(id);
        if (team == null)
        {
            return NotFound();
        }
        return View(team);
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

        var result = await _apiService.CreateTeamAsync(model);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, "Tim oluşturulurken bir hata oluştu.");
        return View(model);
    }

}