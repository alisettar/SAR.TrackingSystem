namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record SectorDistributionResponse(
    List<SectorDistributionItem> Items);

public sealed record SectorDistributionItem(
    string SectorCode,
    string SectorName, 
    int Count);
