using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record MovementRequest(
    Guid VolunteerId,
    Guid? FromSectorId,
    Guid ToSectorId,
    MovementType Type,
    bool IsGroupMovement = false,
    Guid? GroupId = null,
    string? Notes = null);