using SAR.TrackingSystem.Domain.BaseClasses;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Domain.Entities;

public class Movement : Entity
{
    public Guid VolunteerId { get; set; }
    public Guid? FromSectorId { get; set; }                 // null = ilk giriş
    public Guid ToSectorId { get; set; }
    public DateTime MovementTime { get; set; } = DateTime.Now;
    public MovementType Type { get; set; }
    public bool IsGroupMovement { get; set; }
    public Guid? GroupId { get; set; }                      // Grup hareketleri için
    public string? Notes { get; set; }
    
    // Navigation
    public Volunteer Volunteer { get; set; } = null!;
    public Sector? FromSector { get; set; }
    public Sector ToSector { get; set; } = null!;

    public static Movement Create(
        Guid volunteerId,
        Guid? fromSectorId,
        Guid toSectorId,
        MovementType type,
        bool isGroupMovement = false,
        Guid? groupId = null,
        string? notes = null)
    {
        return new Movement
        {
            VolunteerId = volunteerId,
            FromSectorId = fromSectorId,
            ToSectorId = toSectorId,
            MovementTime = DateTime.Now,
            Type = type,
            IsGroupMovement = isGroupMovement,
            GroupId = groupId,
            Notes = notes
        };
    }
}