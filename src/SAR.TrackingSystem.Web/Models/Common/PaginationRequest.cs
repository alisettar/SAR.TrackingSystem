namespace SAR.TrackingSystem.Web.Models.Common;

public sealed record PaginationRequest(
    int Page = 0,
    int PageSize = 10,
    string? SearchText = null,
    string OrderBy = "CreatedAt",
    bool OrderDescending = true);
