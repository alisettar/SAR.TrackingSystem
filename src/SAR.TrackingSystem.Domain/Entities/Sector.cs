using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Sector : Entity
{
    public string Code { get; set; } = string.Empty;        // "BOO", "E-1", "E-2", "ÇIKIŞ"
    public string Name { get; set; } = string.Empty;
    public bool IsEntryPoint { get; set; }                  // BOO = true
    public bool IsExitPoint { get; set; }                   // ÇIKIŞ = true
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public List<Movement> MovementsFrom { get; set; } = new();
    public List<Movement> MovementsTo { get; set; } = new();
}
