using Microsoft.Extensions.Logging;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.DataProcessor.Services;

public class DataCleaningService : IDataCleaningService
{
    private readonly ILogger<DataCleaningService> _logger;
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IMovementRepository _movementRepository;

    public DataCleaningService(
        ILogger<DataCleaningService> logger,
        IVolunteerRepository volunteerRepository,
        IMovementRepository movementRepository)
    {
        _logger = logger;
        _volunteerRepository = volunteerRepository;
        _movementRepository = movementRepository;
    }

    public async Task CleanAllDataAsync()
    {
        _logger.LogInformation("Starting data cleaning process");
        
        await CleanVolunteersAsync();
        await CleanMovementsAsync();
        await RemoveDuplicatesAsync();
        await ValidateDataConsistencyAsync();
        
        _logger.LogInformation("Data cleaning completed");
    }

    public async Task CleanVolunteersAsync()
    {
        _logger.LogInformation("Cleaning volunteer data");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        int cleaned = 0;

        foreach (var volunteer in volunteers.Items)
        {
            bool updated = false;

            // Clean phone numbers
            if (!string.IsNullOrEmpty(volunteer.Phone))
            {
                var cleanPhone = CleanPhoneNumber(volunteer.Phone);
                if (cleanPhone != volunteer.Phone)
                {
                    volunteer.UpdatePhone(cleanPhone);
                    updated = true;
                }
            }

            // Clean names
            if (!string.IsNullOrEmpty(volunteer.FullName))
            {
                var cleanName = CleanName(volunteer.FullName);
                if (cleanName != volunteer.FullName)
                {
                    volunteer.UpdateFullName(cleanName);
                    updated = true;
                }
            }

            if (updated)
            {
                await _volunteerRepository.UpdateAsync(volunteer);
                cleaned++;
            }
        }

        _logger.LogInformation("Cleaned {Count} volunteers", cleaned);
    }

    public async Task CleanMovementsAsync()
    {
        _logger.LogInformation("Cleaning movement data");
        
        var movements = await _movementRepository.GetAllAsync();
        int removed = 0;

        // Remove orphaned movements
        foreach (var movement in movements.Items)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(movement.VolunteerId);
            if (volunteer == null)
            {
                await _movementRepository.DeleteAsync(movement.Id);
                removed++;
            }
        }

        _logger.LogInformation("Removed {Count} orphaned movements", removed);
    }

    public async Task RemoveDuplicatesAsync()
    {
        _logger.LogInformation("Removing duplicate records");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        var duplicates = volunteers.Items
            .GroupBy(v => new { v.TcKimlik, v.FullName })
            .Where(g => g.Count() > 1)
            .SelectMany(g => g.Skip(1))
            .ToList();

        foreach (var duplicate in duplicates)
        {
            await _volunteerRepository.DeleteAsync(duplicate.Id);
        }

        _logger.LogInformation("Removed {Count} duplicate volunteers", duplicates.Count);
    }

    public async Task ValidateDataConsistencyAsync()
    {
        _logger.LogInformation("Validating data consistency");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        int inconsistencies = 0;

        foreach (var volunteer in volunteers.Items)
        {
            var movements = await _movementRepository.GetVolunteerMovementsAsync(volunteer.Id);
            
            // Calculate expected state based on movements
            var expectedState = CalculateExpectedState(movements.ToList());
            
            if (volunteer.CurrentState != expectedState)
            {
                volunteer.UpdateState(expectedState);
                await _volunteerRepository.UpdateAsync(volunteer);
                inconsistencies++;
            }
        }

        _logger.LogInformation("Fixed {Count} state inconsistencies", inconsistencies);
    }

    private string CleanPhoneNumber(string phone)
    {
        // Remove non-numeric characters except +
        var cleaned = string.Concat(phone.Where(c => char.IsDigit(c) || c == '+'));
        
        // Turkish phone number formatting
        if (cleaned.StartsWith("90") && cleaned.Length == 12)
            cleaned = "+" + cleaned;
        else if (cleaned.StartsWith("5") && cleaned.Length == 10)
            cleaned = "+90" + cleaned;
            
        return cleaned;
    }

    private string CleanName(string name)
    {
        return string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }

    private VolunteerState CalculateExpectedState(List<Domain.Entities.Movement> movements)
    {
        if (!movements.Any())
            return VolunteerState.NotEntered;

        var lastMovement = movements.OrderByDescending(m => m.MovementTime).First();
        
        return lastMovement.Type switch
        {
            MovementType.Entry => VolunteerState.InHub,
            MovementType.Transfer => lastMovement.ToSectorId.HasValue ? VolunteerState.InSector : VolunteerState.InHub,
            MovementType.Exit => VolunteerState.Exited,
            MovementType.ReEntry => VolunteerState.InHub,
            _ => VolunteerState.NotEntered
        };
    }
}

public class DataEnrichmentService : IDataEnrichmentService
{
    private readonly ILogger<DataEnrichmentService> _logger;
    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IMovementRepository _movementRepository;

    public DataEnrichmentService(
        ILogger<DataEnrichmentService> logger,
        IVolunteerRepository volunteerRepository,
        IMovementRepository movementRepository)
    {
        _logger = logger;
        _volunteerRepository = volunteerRepository;
        _movementRepository = movementRepository;
    }

    public async Task EnrichAllDataAsync()
    {
        _logger.LogInformation("Starting data enrichment process");
        
        await EnrichVolunteerDataAsync();
        await GenerateQRCodesAsync();
        await UpdateVolunteerStatesAsync();
        await CalculateStatisticsAsync();
        
        _logger.LogInformation("Data enrichment completed");
    }

    public async Task EnrichVolunteerDataAsync()
    {
        _logger.LogInformation("Enriching volunteer data");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        int enriched = 0;

        foreach (var volunteer in volunteers.Items)
        {
            bool updated = false;

            // Add default role if empty
            if (string.IsNullOrEmpty(volunteer.Role))
            {
                volunteer.UpdateRole("Ekip Üyesi");
                updated = true;
            }

            if (updated)
            {
                await _volunteerRepository.UpdateAsync(volunteer);
                enriched++;
            }
        }

        _logger.LogInformation("Enriched {Count} volunteers", enriched);
    }

    public async Task GenerateQRCodesAsync()
    {
        _logger.LogInformation("Generating QR codes for volunteers");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        int generated = 0;

        foreach (var volunteer in volunteers.Items)
        {
            if (string.IsNullOrEmpty(volunteer.QRId))
            {
                var qrId = GenerateQRId(volunteer);
                volunteer.UpdateQRId(qrId);
                await _volunteerRepository.UpdateAsync(volunteer);
                generated++;
            }
        }

        _logger.LogInformation("Generated QR codes for {Count} volunteers", generated);
    }

    public async Task UpdateVolunteerStatesAsync()
    {
        _logger.LogInformation("Updating volunteer states based on movements");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        int updated = 0;

        foreach (var volunteer in volunteers.Items)
        {
            var movements = await _movementRepository.GetVolunteerMovementsAsync(volunteer.Id);
            var currentState = CalculateCurrentState(movements.ToList());
            
            if (volunteer.CurrentState != currentState)
            {
                volunteer.UpdateState(currentState);
                await _volunteerRepository.UpdateAsync(volunteer);
                updated++;
            }
        }

        _logger.LogInformation("Updated states for {Count} volunteers", updated);
    }

    public async Task CalculateStatisticsAsync()
    {
        _logger.LogInformation("Calculating system statistics");
        
        var volunteers = await _volunteerRepository.GetAllAsync();
        var movements = await _movementRepository.GetAllAsync();

        var stats = new
        {
            TotalVolunteers = volunteers.TotalCount,
            InHub = volunteers.Items.Count(v => v.CurrentState == VolunteerState.InHub),
            InSector = volunteers.Items.Count(v => v.CurrentState == VolunteerState.InSector),
            Exited = volunteers.Items.Count(v => v.CurrentState == VolunteerState.Exited),
            TotalMovements = movements.TotalCount
        };

        _logger.LogInformation("Statistics: Total={Total}, InHub={InHub}, InSector={InSector}, Exited={Exited}, Movements={Movements}",
            stats.TotalVolunteers, stats.InHub, stats.InSector, stats.Exited, stats.TotalMovements);
    }

    private string GenerateQRId(Domain.Entities.Volunteer volunteer)
    {
        // Generate QR ID based on TC Kimlik or name
        var seed = !string.IsNullOrEmpty(volunteer.TcKimlik) ? volunteer.TcKimlik : volunteer.FullName;
        return $"SAR{Math.Abs(seed.GetHashCode()):D6}";
    }

    private VolunteerState CalculateCurrentState(List<Domain.Entities.Movement> movements)
    {
        if (!movements.Any())
            return VolunteerState.NotEntered;

        var lastMovement = movements.OrderByDescending(m => m.MovementTime).First();
        
        return lastMovement.Type switch
        {
            MovementType.Entry => VolunteerState.InHub,
            MovementType.Transfer => lastMovement.ToSectorId.HasValue ? VolunteerState.InSector : VolunteerState.InHub,
            MovementType.Exit => VolunteerState.Exited,
            MovementType.ReEntry => VolunteerState.InHub,
            _ => VolunteerState.NotEntered
        };
    }
}
