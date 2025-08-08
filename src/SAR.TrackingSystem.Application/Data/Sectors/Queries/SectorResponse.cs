using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Sectors.Queries;

public sealed record SectorResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsEntryPoint,
    bool IsExitPoint,
    bool IsActive)
{
    public static SectorResponse FromDomain(Sector sector)
    {
        return new SectorResponse(
            sector.Id,
            sector.Code,
            sector.Name,
            sector.IsEntryPoint,
            sector.IsExitPoint,
            sector.IsActive);
    }

    public static List<SectorResponse> FromDomainList(IEnumerable<Sector> sectors)
    {
        return sectors.Select(FromDomain).ToList();
    }
}