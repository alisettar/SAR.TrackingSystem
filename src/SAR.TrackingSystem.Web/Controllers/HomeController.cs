using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IMovementService _movementService;

    public HomeController(IDashboardService dashboardService, IMovementService movementService)
    {
        _dashboardService = dashboardService;
        _movementService = movementService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            var recentMovements = await _movementService.GetRecentMovementsAsync(5);
            
            var dashboardViewModel = new DashboardViewModel
            {
                Stats = stats,
                RecentMovements = recentMovements
            };
            
            return View(dashboardViewModel);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"API bağlantısı başarısız: {ex.Message}";
            return View(new DashboardViewModel());
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData()
    {
        try
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            var recentMovements = await _movementService.GetRecentMovementsAsync(5);
            
            var dashboardViewModel = new DashboardViewModel
            {
                Stats = stats,
                RecentMovements = recentMovements
            };
            
            return PartialView("_DashboardContent", dashboardViewModel);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message });
        }
    }
}
