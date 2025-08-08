using System.ComponentModel.DataAnnotations;

namespace SAR.TrackingSystem.Web.Models;

public class DashboardStats
{
    public long TotalVolunteers { get; set; }
    public int InHubCount { get; set; }
    public int InSectorCount { get; set; }
    public int EntryCount { get; set; }
    public int ExitCount { get; set; }
}

public class DashboardViewModel
{
    public DashboardStats Stats { get; set; } = new();
    public List<MovementViewModel> RecentMovements { get; set; } = new();
}

public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// Volunteer ViewModels - FIXED TO MATCH API
public class VolunteerViewModel
{
    public Guid Id { get; set; }
    public long TcKimlik { get; set; }
    public string FullName { get; set; } = null!;
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public string BloodType { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string EmergencyContactName { get; set; } = null!;
    public string EmergencyContactPhone { get; set; } = null!;
    public string? Buddy1 { get; set; }
    public string? Buddy2 { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VolunteerCreateViewModel
{
    [Required(ErrorMessage = "TC Kimlik zorunludur")]
    [Range(10000000000, 99999999999, ErrorMessage = "TC Kimlik 11 haneli olmalıdır")]
    public long TcKimlik { get; set; }
    
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arası olmalıdır")]
    public string FullName { get; set; } = null!;
    
    [Required(ErrorMessage = "Tim seçimi zorunludur")]
    public Guid TeamId { get; set; }
    
    [Required(ErrorMessage = "Kan grubu zorunludur")]
    public string BloodType { get; set; } = null!;
    
    [Required(ErrorMessage = "Telefon zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Phone { get; set; } = null!;
    
    [Required(ErrorMessage = "Acil durum kişisi zorunludur")]
    public string EmergencyContactName { get; set; } = null!;
    
    [Required(ErrorMessage = "Acil durum telefonu zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string EmergencyContactPhone { get; set; } = null!;
    
    public string? Buddy1 { get; set; }
    public string? Buddy2 { get; set; }
}

public class VolunteerUpdateViewModel
{
    [Required(ErrorMessage = "TC Kimlik zorunludur")]
    [Range(10000000000, 99999999999, ErrorMessage = "TC Kimlik 11 haneli olmalıdır")]
    public long TcKimlik { get; set; }
    
    [Required(ErrorMessage = "Ad Soyad zorunludur")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arası olmalıdır")]
    public string FullName { get; set; } = null!;
    
    [Required(ErrorMessage = "Tim seçimi zorunludur")]
    public Guid TeamId { get; set; }
    
    [Required(ErrorMessage = "Kan grubu zorunludur")]
    public string BloodType { get; set; } = null!;
    
    [Required(ErrorMessage = "Telefon zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Phone { get; set; } = null!;
    
    [Required(ErrorMessage = "Acil durum kişisi zorunludur")]
    public string EmergencyContactName { get; set; } = null!;
    
    [Required(ErrorMessage = "Acil durum telefonu zorunludur")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string EmergencyContactPhone { get; set; } = null!;
    
    public string? Buddy1 { get; set; }
    public string? Buddy2 { get; set; }
}

// Movement ViewModels - FIXED TO MATCH API
public class MovementViewModel
{
    public Guid Id { get; set; }
    public string VolunteerName { get; set; } = null!;
    public string? FromSectorName { get; set; }
    public string ToSectorName { get; set; } = null!;
    public DateTime MovementTime { get; set; }
    public string MovementType { get; set; } = null!;
    public bool IsGroupMovement { get; set; }
    public Guid? GroupId { get; set; }
    public string? Notes { get; set; }
}

public class MovementCreateViewModel
{
    [Required(ErrorMessage = "Gönüllü seçimi zorunludur")]
    public Guid VolunteerId { get; set; }
    
    public Guid? FromSectorId { get; set; }
    
    [Required(ErrorMessage = "Hedef sektör seçimi zorunludur")]
    public Guid ToSectorId { get; set; }
    
    [Required(ErrorMessage = "Hareket tipi zorunludur")]
    public int Type { get; set; } = 0; // 0=Entry, 1=Transfer, 2=Exit
    
    public bool IsGroupMovement { get; set; }
    
    [RequiredIf("IsGroupMovement", true, ErrorMessage = "Grup hareketi için Grup ID zorunludur")]
    public Guid? GroupId { get; set; }
    
    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olmalıdır")]
    public string? Notes { get; set; }
}

// Dropdown ViewModels - FIXED TO MATCH API
public class TeamViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int Capacity { get; set; }
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
