using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Sector : Entity
{
    public string Code { get; set; } = string.Empty;        // "BoO", "E-1", "E-2", "Exit"
    public string Name { get; set; } = string.Empty;
    public bool IsEntryPoint { get; set; }                  // BoO = true
    public bool IsExitPoint { get; set; }                   // Exit = true
    public bool IsActive { get; set; } = true;
    public bool IsCriticalForBusinessRules { get; set; }    // Entry, BoO, Exit = true
    
    // Work Area Properties
    public string WorkAreaName { get; set; } = string.Empty;        // Çalışma alanı adı
    public string WorkAreaAddress { get; set; } = string.Empty;      // Çalışma alanı adresi
    public string Coordinates { get; set; } = string.Empty;          // NoorEinat (enlem boylam)
    public int WorkAreaNumber { get; set; } = 0;                     // Çalışma Alanı no
    public int ExpectedVictimCount { get; set; } = 0;                // Beklenen Afetzede sayısı

    /// <summary>
    /// BUSİNESS CRİTİCAL: Bu sektörler SAR operasyon kuralları için kritiktir ve SİLİNMEMELİDİR:
    /// - Entry: İlk giriş noktası (Entry rule)
    /// - BoO: Hub sektör (Transfer rule) 
    /// - Exit: Çıkış noktası (Exit rule)
    /// 
    /// Bu sektörlerin silinmesi sistem business rule'larını bozar.
    /// Delete işlemlerinde bu kontroller yapılmalıdır.
    /// </summary>

    // Navigation
    public List<Movement> MovementsFrom { get; set; } = new();
    public List<Movement> MovementsTo { get; set; } = new();

    public Sector()
    {
        Id = Guid.NewGuid();
    }

    public Sector(string code, string name, bool isActive = true, bool isEntryPoint = false, bool isExitPoint = false, bool isCriticalForBusinessRules = false,
                  string workAreaName = "", string workAreaAddress = "", string coordinates = "", int workAreaNumber = 0, int expectedVictimCount = 0)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        IsActive = isActive;
        IsEntryPoint = isEntryPoint;
        IsExitPoint = isExitPoint;
        IsCriticalForBusinessRules = isCriticalForBusinessRules;
        WorkAreaName = workAreaName;
        WorkAreaAddress = workAreaAddress;
        Coordinates = coordinates;
        WorkAreaNumber = workAreaNumber;
        ExpectedVictimCount = expectedVictimCount;
    }
}
