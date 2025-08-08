using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Volunteer : Entity
{
    public long TcKimlik { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string? Buddy1 { get; set; }
    public string? Buddy2 { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation
    public Team Team { get; set; } = null!;
    public List<Movement> Movements { get; set; } = new();
}
