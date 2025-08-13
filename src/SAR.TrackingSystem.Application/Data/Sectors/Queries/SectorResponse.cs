using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Sectors.Queries;

public sealed record SectorResponse(
    Guid Id,
    string Code,
    string Name,
    bool IsEntryPoint,
    bool IsExitPoint,
    bool IsActive,
    string WorkAreaName,
    string WorkAreaAddress,
    string Coordinates,
    int WorkAreaNumber,
    int ExpectedVictimCount)
{
    public static SectorResponse FromDomain(Sector sector)
    {
        return new SectorResponse(
            sector.Id,
            sector.Code,
            sector.Name,
            sector.IsEntryPoint,
            sector.IsExitPoint,
            sector.IsActive,
            sector.WorkAreaName,
            sector.WorkAreaAddress,
            sector.Coordinates,
            sector.WorkAreaNumber,
            sector.ExpectedVictimCount);
    }

    public static List<SectorResponse> FromDomainList(IEnumerable<Sector> sectors)
    {
        return sectors.Select(FromDomain).ToList();
    }
}