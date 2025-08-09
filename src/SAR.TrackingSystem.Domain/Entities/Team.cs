using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Team : Entity
{
    public string Name { get; set; } = string.Empty;        // "A TİMİ", "MEDİKAL", "LOJİSTİK"
    public string Code { get; set; } = string.Empty;        // "A", "MED", "LOG"
    public string? City { get; set; }                       // Şehir bilgisi (opsiyonel)
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public List<Volunteer> Volunteers { get; set; } = new();

    public Team()
    {
        Id = Guid.NewGuid();
    }

    public Team(string code, string name, string? city = null, bool isActive = true)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        City = city;
        IsActive = isActive;
    }
}
