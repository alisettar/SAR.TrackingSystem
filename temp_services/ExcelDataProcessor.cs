using Microsoft.Extensions.Logging;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;
using SAR.TrackingSystem.Application.Repositories;
using OfficeOpenXml;

namespace SAR.TrackingSystem.DataProcessor.Services;

public class ExcelDataProcessor : IExcelDataProcessor
{
    private readonly ILogger<ExcelDataProcessor> _logger;
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IMovementRepository _movementRepository;
    private readonly ISectorRepository _sectorRepository;

    public ExcelDataProcessor(
        ILogger<ExcelDataProcessor> logger,
        IVolunteerRepository volunteerRepository,
        ITeamRepository teamRepository,
        IMovementRepository movementRepository,
        ISectorRepository sectorRepository)
    {
        _logger = logger;
        _volunteerRepository = volunteerRepository;
        _teamRepository = teamRepository;
        _movementRepository = movementRepository;
        _sectorRepository = sectorRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task ImportFromExcelAsync(string filePath)
    {
        _logger.LogInformation("Starting Excel import from: {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Excel file not found: {filePath}");
        }

        var volunteers = await ReadVolunteersFromExcelAsync(filePath);
        _logger.LogInformation("Read {Count} volunteers from Excel", volunteers.Count);

        // Save volunteers to database
        foreach (var volunteer in volunteers)
        {
            await _volunteerRepository.CreateAsync(volunteer);
        }

        _logger.LogInformation("Successfully imported {Count} volunteers", volunteers.Count);
    }

    public async Task ExportToExcelAsync(string filePath)
    {
        _logger.LogInformation("Starting Excel export to: {FilePath}", filePath);

        var volunteers = await _volunteerRepository.GetAllAsync();
        var movements = await _movementRepository.GetAllAsync();

        await WriteVolunteersToExcelAsync(volunteers.Items.ToList(), filePath);
        
        _logger.LogInformation("Successfully exported {Count} volunteers to Excel", volunteers.Items.Count());
    }

    public async Task<List<Volunteer>> ReadVolunteersFromExcelAsync(string filePath)
    {
        var volunteers = new List<Volunteer>();
        
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        
        if (worksheet == null)
        {
            _logger.LogWarning("No worksheet found in Excel file");
            return volunteers;
        }

        var teams = await _teamRepository.GetAllAsync();
        var teamLookup = teams.Items.ToDictionary(t => t.Name, t => t.Id);

        // Assuming Excel format: TC, FullName, TeamName, Role, BloodType, Phone
        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            try
            {
                var tcKimlik = worksheet.Cells[row, 1].Value?.ToString();
                var fullName = worksheet.Cells[row, 2].Value?.ToString();
                var teamName = worksheet.Cells[row, 3].Value?.ToString();
                var role = worksheet.Cells[row, 4].Value?.ToString();
                var bloodType = worksheet.Cells[row, 5].Value?.ToString();
                var phone = worksheet.Cells[row, 6].Value?.ToString();

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(teamName))
                    continue;

                if (!teamLookup.TryGetValue(teamName, out var teamId))
                {
                    _logger.LogWarning("Team not found: {TeamName} for volunteer {FullName}", teamName, fullName);
                    continue;
                }

                var volunteer = Volunteer.Create(
                    tcKimlik ?? string.Empty,
                    fullName,
                    teamId,
                    role ?? string.Empty,
                    bloodType ?? string.Empty,
                    phone ?? string.Empty,
                    string.Empty,
                    string.Empty
                );

                volunteers.Add(volunteer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading volunteer from row {Row}", row);
            }
        }

        return volunteers;
    }

    public async Task<List<Team>> ReadTeamsFromExcelAsync(string filePath)
    {
        // Implementation for reading teams from Excel
        return new List<Team>();
    }

    public async Task<List<Movement>> ReadMovementsFromExcelAsync(string filePath)
    {
        // Implementation for reading movements from Excel
        return new List<Movement>();
    }

    public async Task WriteVolunteersToExcelAsync(List<Volunteer> volunteers, string filePath)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Volunteers");

        // Headers
        worksheet.Cells[1, 1].Value = "TC Kimlik";
        worksheet.Cells[1, 2].Value = "Ad Soyad";
        worksheet.Cells[1, 3].Value = "Ekip";
        worksheet.Cells[1, 4].Value = "Görev";
        worksheet.Cells[1, 5].Value = "Kan Grubu";
        worksheet.Cells[1, 6].Value = "Telefon";
        worksheet.Cells[1, 7].Value = "QR ID";
        worksheet.Cells[1, 8].Value = "Durum";

        var teams = await _teamRepository.GetAllAsync();
        var teamLookup = teams.Items.ToDictionary(t => t.Id, t => t.Name);

        // Data
        for (int i = 0; i < volunteers.Count; i++)
        {
            var volunteer = volunteers[i];
            int row = i + 2;

            worksheet.Cells[row, 1].Value = volunteer.TcKimlik;
            worksheet.Cells[row, 2].Value = volunteer.FullName;
            worksheet.Cells[row, 3].Value = teamLookup.GetValueOrDefault(volunteer.TeamId, "Unknown");
            worksheet.Cells[row, 4].Value = volunteer.Role;
            worksheet.Cells[row, 5].Value = volunteer.BloodType;
            worksheet.Cells[row, 6].Value = volunteer.Phone;
            worksheet.Cells[row, 7].Value = volunteer.QRId;
            worksheet.Cells[row, 8].Value = volunteer.CurrentState.ToString();
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        // Save file
        await package.SaveAsAsync(new FileInfo(filePath));
    }

    public async Task WriteMovementsToExcelAsync(List<Movement> movements, string filePath)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Movements");

        // Headers
        worksheet.Cells[1, 1].Value = "Volunteer";
        worksheet.Cells[1, 2].Value = "From Sector";
        worksheet.Cells[1, 3].Value = "To Sector";
        worksheet.Cells[1, 4].Value = "Movement Time";
        worksheet.Cells[1, 5].Value = "Type";
        worksheet.Cells[1, 6].Value = "Group Movement";
        worksheet.Cells[1, 7].Value = "Notes";

        var volunteers = await _volunteerRepository.GetAllAsync();
        var sectors = await _sectorRepository.GetAllAsync();
        
        var volunteerLookup = volunteers.Items.ToDictionary(v => v.Id, v => v.FullName);
        var sectorLookup = sectors.Items.ToDictionary(s => s.Id, s => s.Name);

        // Data
        for (int i = 0; i < movements.Count; i++)
        {
            var movement = movements[i];
            int row = i + 2;

            worksheet.Cells[row, 1].Value = volunteerLookup.GetValueOrDefault(movement.VolunteerId, "Unknown");
            worksheet.Cells[row, 2].Value = movement.FromSectorId.HasValue ? 
                sectorLookup.GetValueOrDefault(movement.FromSectorId.Value, "Unknown") : "Entry";
            worksheet.Cells[row, 3].Value = movement.ToSectorId.HasValue ? 
                sectorLookup.GetValueOrDefault(movement.ToSectorId.Value, "Unknown") : "Exit";
            worksheet.Cells[row, 4].Value = movement.MovementTime.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cells[row, 5].Value = movement.Type.ToString();
            worksheet.Cells[row, 6].Value = movement.IsGroupMovement ? "Yes" : "No";
            worksheet.Cells[row, 7].Value = movement.Notes;
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        // Save file
        await package.SaveAsAsync(new FileInfo(filePath));
    }
}
