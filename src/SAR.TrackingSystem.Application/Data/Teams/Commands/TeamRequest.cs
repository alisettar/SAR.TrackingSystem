namespace SAR.TrackingSystem.Application.Data.Teams.Commands;

public sealed record TeamRequest(
    string Code,
    string Name,
    string? City = null,
    bool IsActive = true);