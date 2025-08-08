using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Data.Movements.Queries;

public sealed record MovementResponse(
    Guid Id,
    Guid VolunteerId,
    string VolunteerName,
    Guid? FromSectorId,
    string? FromSectorName,
    Guid ToSectorId,
    string ToSectorName,
    DateTime MovementTime,
    MovementType Type,
    bool IsGroupMovement,
    Guid? GroupId,
    string? Notes)
{
    public static MovementResponse FromDomain(Movement movement)
    {
        return new MovementResponse(
            movement.Id,
            movement.VolunteerId,
            movement.Volunteer.FullName,
            movement.FromSectorId,
            movement.FromSector?.Name,
            movement.ToSectorId,
            movement.ToSector.Name,
            movement.MovementTime,
            movement.Type,
            movement.IsGroupMovement,
            movement.GroupId,
            movement.Notes);
    }

    public static List<MovementResponse> FromDomainList(IEnumerable<Movement> movements)
    {
        return movements.Select(FromDomain).ToList();
    }
}