namespace SAR.TrackingSystem.Application.Data.Sectors.Commands;

public sealed record SectorRequest(
    string Code,
    string Name,
    bool IsActive = true,
    bool IsEntryPoint = false,
    bool IsExitPoint = false);