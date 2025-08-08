using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Team : Entity
{
    public string Name { get; set; } = string.Empty;        // "A TİMİ", "MEDİKAL", "LOJİSTİK"
    public string Code { get; set; } = string.Empty;        // "A", "MED", "LOG"
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public List<Volunteer> Volunteers { get; set; } = new();
}
