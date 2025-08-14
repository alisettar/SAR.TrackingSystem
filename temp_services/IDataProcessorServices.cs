using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.DataProcessor.Services;

public interface IExcelDataProcessor
{
    Task ImportFromExcelAsync(string filePath);
    Task ExportToExcelAsync(string filePath);
    Task<List<Volunteer>> ReadVolunteersFromExcelAsync(string filePath);
    Task<List<Team>> ReadTeamsFromExcelAsync(string filePath);
    Task<List<Movement>> ReadMovementsFromExcelAsync(string filePath);
    Task WriteVolunteersToExcelAsync(List<Volunteer> volunteers, string filePath);
    Task WriteMovementsToExcelAsync(List<Movement> movements, string filePath);
}

public interface IDataCleaningService
{
    Task CleanAllDataAsync();
    Task CleanVolunteersAsync();
    Task CleanMovementsAsync();
    Task RemoveDuplicatesAsync();
    Task ValidateDataConsistencyAsync();
}

public interface IDataEnrichmentService
{
    Task EnrichAllDataAsync();
    Task EnrichVolunteerDataAsync();
    Task GenerateQRCodesAsync();
    Task CalculateStatisticsAsync();
    Task UpdateVolunteerStatesAsync();
}
