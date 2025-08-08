namespace SAR.TrackingSystem.Application.Data;

public sealed class PaginationResponse<T>(
    IEnumerable<T> items,
    long totalCount)
{
    public IEnumerable<T> Items { get; } = items;
    public long TotalCount { get; } = totalCount;
}