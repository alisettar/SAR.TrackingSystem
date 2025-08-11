namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record CityDistributionResponse(
    List<CityDistributionItem> Items);

public sealed record CityDistributionItem(
    string CityName,
    int Count);
