namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public record TeamDistributionResponse(List<TeamDistributionItem> Items);

public record TeamDistributionItem(
    string TeamName,
    string City,
    int ArrivedCount,
    int TotalCount
);
