namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record DashboardStatsResponse(
    long TotalVolunteers,
    int NonArrivedCount,
    int InHubCount,
    int InSectorCount,
    int EntryCount,
    int ExitCount,
    int TotalExpectedVictims,
    int TotalRescuedCount,
    int TotalExtricatedCount,
    List<SectorMapResponse> Sectors);

public sealed record SectorMapResponse(
    string Code,
    string Name,
    string Coordinates,
    int RescuedCount,
    int ExtricatedCount,
    int ExpectedVictimCount,
    string WorkAreaName);
