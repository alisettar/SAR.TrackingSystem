using System.ComponentModel.DataAnnotations;
using SAR.TrackingSystem.Web.Services;

namespace SAR.TrackingSystem.Web.Models;

public class DashboardStats
{
    public long TotalVolunteers { get; set; }
    public int InHubCount { get; set; }
    public int InSectorCount { get; set; }
    public int EntryCount { get; set; }
    public int ExitCount { get; set; }
    public int NonArrivedCount { get; set; }
}

public class DashboardViewModel
{
    public DashboardStats Stats { get; set; } = new();
    public List<MovementViewModel> RecentMovements { get; set; } = new();
    public List<VolunteerViewModel> NonArrivedVolunteers { get; set; } = new();
    public SectorDistributionData SectorDistribution { get; set; } = new();
    public CityDistributionData CityDistribution { get; set; } = new();
    public TeamDistributionData TeamDistribution { get; set; } = new();
}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// Volunteer ViewModels - UPDATED TO NEW SCHEMA
public class VolunteerViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string QRId { get; set; } = null!;
    public string Role { get; set; } = null!;
    public int CurrentState { get; set; }
    
    // Display property for CurrentState
    public string StateDisplay => CurrentState switch
    {
        0 => "Gelmedi",
        1 => "BoO'da", 
        2 => "Sektörde",
        3 => "Çıkış Yaptı",
        _ => "Bilinmiyor"
    };
    
    public string StateBadgeClass => CurrentState switch
    {
        0 => "bg-secondary",
        1 => "bg-success",
        2 => "bg-warning", 
        3 => "bg-info",
        _ => "bg-dark"
    };
}

public class VolunteerCreateViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-200 karakter arası olmalıdır")]
    public string FullName { get; set; } = null!;
    
    [Required(ErrorMessage = "Ekip seçimi zorunludur")]
    public Guid TeamId { get; set; }
    
    [Required(ErrorMessage = "Görev zorunludur")]
    [StringLength(100, ErrorMessage = "Görev en fazla 100 karakter olmalıdır")]
    public string Role { get; set; } = null!;
    
    [Required(ErrorMessage = "QR ID zorunludur")]
    [StringLength(50, ErrorMessage = "QR ID en fazla 50 karakter olmalıdır")]
    public string QRId { get; set; } = null!;
}

public class VolunteerUpdateViewModel
{
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-200 karakter arası olmalıdır")]
    public string FullName { get; set; } = null!;
    
    [Required(ErrorMessage = "Ekip seçimi zorunludur")]
    public Guid TeamId { get; set; }
    
    [Required(ErrorMessage = "Görev zorunludur")]
    [StringLength(100, ErrorMessage = "Görev en fazla 100 karakter olmalıdır")]
    public string Role { get; set; } = null!;
    
    [Required(ErrorMessage = "QR ID zorunludur")]
    [StringLength(50, ErrorMessage = "QR ID en fazla 50 karakter olmalıdır")]
    public string QRId { get; set; } = null!;
}

// Movement ViewModels - FIXED TO MATCH API
public class MovementViewModel
{
    public Guid Id { get; set; }
    public string VolunteerName { get; set; } = null!;
    public string TeamName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? FromSectorName { get; set; }
    public string ToSectorName { get; set; } = null!;
    public DateTime MovementTime { get; set; }
    public string MovementType { get; set; } = null!;
    public bool IsGroupMovement { get; set; }
    public Guid? GroupId { get; set; }
    public string? Notes { get; set; }
    
    // Display properties for null sectors (State Machine)
    public string FromSectorDisplay => string.IsNullOrEmpty(FromSectorName) ? "Alan Dışı" : FromSectorName;
    public string ToSectorDisplay => string.IsNullOrEmpty(ToSectorName) ? "Alan Dışı" : ToSectorName;
}

public class MovementCreateViewModel
{
    public Guid VolunteerId { get; set; }
    
    public Guid? FromSectorId { get; set; }
    
    public Guid? ToSectorId { get; set; }
    
    public int Type { get; set; } = 0; // 0=Entry, 1=Transfer, 2=Exit
    
    public bool IsGroupMovement { get; set; }
    
    [RequiredIf("IsGroupMovement", true, ErrorMessage = "Grup hareketi için Grup ID zorunludur")]
    public Guid? GroupId { get; set; }
    
    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olmalıdır")]
    public string? Notes { get; set; }
    
    // QR Operations properties
    public string? QRId { get; set; }
    public bool IsExit { get; set; }
    public bool ReturnToHub { get; set; }
}

// Team Movement ViewModel for group operations
public class TeamMovementCreateViewModel
{
    [Required(ErrorMessage = "Ekip seçimi zorunludur")]
    public Guid TeamId { get; set; }
    
    // State Machine: Nullable sectors
    public Guid? FromSectorId { get; set; }
    
    public Guid? ToSectorId { get; set; }
    
    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olmalıdır")]
    public string? Notes { get; set; }
    
    // Auto-generated properties
    public int Type { get; set; }
    public bool IsGroupMovement => true;
    public Guid GroupId { get; set; } = Guid.NewGuid();
}

// Dropdown ViewModels - FIXED TO MATCH API
public class TeamViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? City { get; set; }
}

public class TeamDetailsViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string? City { get; set; }
    public List<TeamMemberViewModel> Members { get; set; } = new();
}

public class TeamMemberViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? QRId { get; set; }
    public string? Role { get; set; }
}

public class SectorViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsEntryPoint { get; set; }
    public bool IsExitPoint { get; set; }
    public bool IsActive { get; set; }
}
