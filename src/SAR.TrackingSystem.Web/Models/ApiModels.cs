namespace SAR.TrackingSystem.Web.Models;

// API Response Types - Match actual API structure
public class PaginationResponse<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public long TotalCount { get; set; }
}
