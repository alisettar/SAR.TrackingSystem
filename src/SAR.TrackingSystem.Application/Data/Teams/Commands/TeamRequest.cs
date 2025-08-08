namespace SAR.TrackingSystem.Application.Data.Teams.Commands;

public sealed record TeamRequest(
    string Code,
    string Name,
    int Capacity,
    bool IsActive = true);