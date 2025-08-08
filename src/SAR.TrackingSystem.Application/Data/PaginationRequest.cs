using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SAR.TrackingSystem.Application.Data;

public sealed record PaginationRequest(
    int Page = 0,
    int PageSize = 10,
    string? SearchText = null,
    string OrderBy = "CreatedAt",
    bool OrderDescending = true) : IParsable<PaginationRequest>
{
    public static PaginationRequest Parse(string s, IFormatProvider? provider)
    {
        return JsonSerializer.Deserialize<PaginationRequest>(s) ?? new();
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out PaginationRequest result)
    {
        result = null;
        try
        {
            result = (s != null ? JsonSerializer.Deserialize<PaginationRequest>(s) : new()) ?? new();
            return true;
        }
        catch
        {
            return false;
        }
    }
}