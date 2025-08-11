namespace SAR.TrackingSystem.Domain.Configuration;

public class SectorConfiguration
{
    public const string SectionName = "SectorSettings";
    public List<string> CriticalSectorCodes { get; set; } = [];

    public string HubCode { get; set; } = "BoO";
}