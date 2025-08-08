namespace SAR.TrackingSystem.Domain.Configuration;

public class SectorConfiguration
{
    public const string SectionName = "SectorSettings";
    public List<string> CriticalSectorCodes { get; set; } = new List<string>();

    public string EntryCode { get; set; } = "ALAN_DIŞI";
    public string HubCode { get; set; } = "BOO";
    public string ExitCode { get; set; } = "ÇIKIŞ";
}