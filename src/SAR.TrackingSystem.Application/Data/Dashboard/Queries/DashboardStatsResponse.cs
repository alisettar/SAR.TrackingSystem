namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record DashboardStatsResponse(
    long TotalVolunteers,
    int NonArrivedCount,
    int InHubCount,
    int InSectorCount,
    int EntryCount,
    int ExitCount);
