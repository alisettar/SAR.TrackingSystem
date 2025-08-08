using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly ISarApiService _apiService;

    public HomeController(ISarApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await _apiService.GetDashboardStatsAsync();
            return View(stats);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"API bağlantısı başarısız: {ex.Message}";
            return View();
        }
    }
}
