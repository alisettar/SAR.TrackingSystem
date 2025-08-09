using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Controllers;

public class MovementsController : Controller
{
    private readonly IMovementService _movementService;
    private readonly ITeamService _teamService;
    private readonly ISectorService _sectorService;
    private readonly IVolunteerService _volunteerService;
    private readonly ILogger<MovementsController> _logger;

    public MovementsController(IMovementService movementService, ITeamService teamService, 
                              ISectorService sectorService, IVolunteerService volunteerService, ILogger<MovementsController> logger)
    {
        _movementService = movementService;
        _teamService = teamService;
        _sectorService = sectorService;
        _volunteerService = volunteerService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var request = new PaginationRequest(page - 1, 20, search); // Convert to 0-based
        var movements = await _movementService.GetMovementsAsync(request);
        ViewBag.SearchTerm = search;
        return View(movements);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Teams = await _teamService.GetTeamsAsync();
        ViewBag.Sectors = await _sectorService.GetSectorsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TeamMovementCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teams = await _teamService.GetTeamsAsync();
            ViewBag.Sectors = await _sectorService.GetSectorsAsync();
            return View(model);
        }

        try
        {
            await _movementService.CreateTeamMovementAsync(model);
            TempData["Success"] = "Ekip hareket kaydı başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Hata: {ex.Message}";
            ViewBag.Teams = await _teamService.GetTeamsAsync();
            ViewBag.Sectors = await _sectorService.GetSectorsAsync();
            return View(model);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _movementService.DeleteMovementAsync(id);
            if (success)
                TempData["Success"] = "Hareket başarıyla silindi.";
            else
                TempData["Error"] = "Hareket silinemedi.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Hata: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
    
    // API Endpoint for team members (used by JavaScript)
    [HttpGet("api/teams/{teamId}/members")]
    public async Task<IActionResult> GetTeamMembers(Guid teamId)
    {
        try
        {
            var members = await _teamService.GetTeamMembersListAsync(teamId);
            return Json(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team members for team {TeamId}", teamId);
            return Json(new List<TeamMemberViewModel>());
        }
    }
    
    public async Task<IActionResult> QuickEntry()
    {
        ViewBag.Sectors = await _sectorService.GetSectorsAsync();
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> QuickEntryOperation(string operationType, string qrId, Guid? targetSectorId = null)
    {
        if (string.IsNullOrWhiteSpace(qrId))
        {
            return Json(new { success = false, message = "QR ID giriniz" });
        }

        try
        {
            var result = operationType switch
            {
                "entry" => await ProcessQuickEntry(qrId.Trim()),
                "exit" => await ProcessQuickExit(qrId.Trim()), 
                "sector" => await ProcessSectorTransfer(qrId.Trim(), targetSectorId),
                "hub" => await ProcessReturnToHub(qrId.Trim()),
                _ => (false, "❌ Geçersiz işlem türü")
            };
            
            return Json(new { success = result.Item1, message = result.Item2 });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QuickEntry {Operation} error for {QRId}", operationType, qrId);
            return Json(new { success = false, message = $"❌ Sistem hatası: {ex.Message}" });
        }
    }
    
    private async Task<(bool success, string message)> ProcessQuickEntry(string qrId)
    {
        try
        {
            var success = await _movementService.CreateQuickEntryAsync(qrId);
            return success 
                ? (true, $"✅ {qrId} - Alana giriş kaydedildi")
                : (false, $"❌ {qrId} - QR ID bulunamadı veya giriş kurallarına uymuyor");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            return (false, $"❌ {qrId} - Giriş kurallarına uymuyor");
        }
    }
    
    private async Task<(bool success, string message)> ProcessQuickExit(string qrId)
    {
        try
        {
            // Find volunteer by QR ID
            var request = new PaginationRequest(0, 1000, qrId);
            var volunteers = await _volunteerService.GetVolunteersAsync(request);
            var volunteer = volunteers.Items.FirstOrDefault(v => v.QRId == qrId);
            
            if (volunteer == null)
                return (false, $"❌ {qrId} - QR ID bulunamadı");
                
            // Get sectors for exit (BoO → ÇIKIŞ)
            var sectors = await _sectorService.GetSectorsAsync();
            var hubSector = sectors.FirstOrDefault(s => s.Code == "BoO");
            var exitSector = sectors.FirstOrDefault(s => s.Code == "ÇIKIŞ");
            
            if (hubSector == null || exitSector == null)
                return (false, $"❌ {qrId} - Sistem hatası: Gerekli sektörler bulunamadı");
            
            var model = new MovementCreateViewModel
            {
                VolunteerId = volunteer.Id,
                FromSectorId = hubSector.Id,
                ToSectorId = exitSector.Id,
                Type = 2, // Exit
                Notes = $"QR Çıkış: {qrId}"
            };
            
            await _movementService.CreateMovementAsync(model);
            return (true, $"✅ {qrId} - Alandan çıkış kaydedildi");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            return (false, $"❌ {qrId} - Çıkış kurallarına uymuyor (BoO'da değil)");
        }
    }
    
    private async Task<(bool success, string message)> ProcessSectorTransfer(string qrId, Guid? targetSectorId)
    {
        if (!targetSectorId.HasValue)
            return (false, "❌ Hedef sektör seçiniz");
        
        try
        {
            // Find volunteer by QR ID
            var request = new PaginationRequest(0, 1000, qrId);
            var volunteers = await _volunteerService.GetVolunteersAsync(request);
            var volunteer = volunteers.Items.FirstOrDefault(v => v.QRId == qrId);
            
            if (volunteer == null)
                return (false, $"❌ {qrId} - QR ID bulunamadı");
                
            // Get sectors
            var sectors = await _sectorService.GetSectorsAsync();
            var hubSector = sectors.FirstOrDefault(s => s.Code == "BoO");
            var targetSector = sectors.FirstOrDefault(s => s.Id == targetSectorId.Value);
            
            if (hubSector == null || targetSector == null)
                return (false, $"❌ {qrId} - Sektör bulunamadı");
            
            var model = new MovementCreateViewModel
            {
                VolunteerId = volunteer.Id,
                FromSectorId = hubSector.Id,
                ToSectorId = targetSectorId.Value,
                Type = 1, // Transfer
                Notes = $"QR Sektöre: {qrId} → {targetSector.Name}"
            };
            
            await _movementService.CreateMovementAsync(model);
            return (true, $"✅ {qrId} - {targetSector.Name}'ya gönderildi");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            return (false, $"❌ {qrId} - Sektöre gönderme kurallarına uymuyor (BoO'da değil)");
        }
    }
    
    private async Task<(bool success, string message)> ProcessReturnToHub(string qrId)
    {
        try
        {
            // Find volunteer by QR ID
            var volunteerRequest = new PaginationRequest(0, 1000, qrId);
            var volunteers = await _volunteerService.GetVolunteersAsync(volunteerRequest);
            var volunteer = volunteers.Items.FirstOrDefault(v => v.QRId == qrId);
            
            if (volunteer == null)
                return (false, $"❌ {qrId} - QR ID bulunamadı");
                
            // Get last movement to determine source sector
            var movementRequest = new PaginationRequest(0, 50);
            var movements = await _movementService.GetMovementsAsync(movementRequest);
            var lastMovement = movements.Items
                .Where(m => m.VolunteerName.Contains(volunteer.FullName))
                .OrderByDescending(m => m.MovementTime)
                .FirstOrDefault();
                
            if (lastMovement == null)
                return (false, $"❌ {qrId} - Hareket geçmişi bulunamadı");
                
            // Get sectors
            var sectors = await _sectorService.GetSectorsAsync();
            var hubSector = sectors.FirstOrDefault(s => s.Code == "BoO");
            var fromSector = sectors.FirstOrDefault(s => s.Name == lastMovement.ToSectorName);
            
            if (hubSector == null || fromSector == null || fromSector.Code == "BoO")
                return (false, $"❌ {qrId} - BoO'ya dönüş için bir sektörde olmalısınız");
            
            var model = new MovementCreateViewModel
            {
                VolunteerId = volunteer.Id,
                FromSectorId = fromSector.Id,
                ToSectorId = hubSector.Id,
                Type = 1, // Transfer
                Notes = $"QR BoO'ya Dönüş: {qrId} ← {fromSector.Name}"
            };
            
            await _movementService.CreateMovementAsync(model);
            return (true, $"✅ {qrId} - BoO'ya dönüş kaydedildi");
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            return (false, $"❌ {qrId} - BoO'ya dönüş kurallarına uymuyor (sektörde değil)");
        }
    }
}
