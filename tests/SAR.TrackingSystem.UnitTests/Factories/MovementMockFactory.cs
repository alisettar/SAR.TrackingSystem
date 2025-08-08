using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class MovementMockFactory
{
    private static readonly Random Random = new();
    
    public static List<Movement> GetSampleMovements(List<Volunteer> volunteers, List<Sector> sectors)
    {
        var movements = new List<Movement>();
        
        var alanDisi = sectors.First(s => s.Code == "ALAN_DIŞI");
        var boo = sectors.First(s => s.Code == "BOO");
        var cikis = sectors.First(s => s.Code == "ÇIKIŞ");
        var operationalSectors = sectors.Where(s => s.Code != "ALAN_DIŞI" && s.Code != "BOO" && s.Code != "ÇIKIŞ").ToList();
        
        // Generate movements for random volunteers
        var selectedVolunteers = volunteers.OrderBy(x => Random.Next()).Take(800).ToList();
        
        var baseTime = DateTime.Now.AddDays(-30); // Son 30 gün
        
        foreach (var volunteer in selectedVolunteers)
        {
            var volunteerMovements = GenerateVolunteerMovementChain(volunteer.Id, alanDisi, boo, cikis, operationalSectors, baseTime);
            movements.AddRange(volunteerMovements);
            
            // Her gönüllü için farklı zaman dilimi
            baseTime = baseTime.AddHours(Random.Next(1, 6));
        }
        
        // Ensure exactly 10000 movements
        while (movements.Count < 10000)
        {
            var randomVolunteer = volunteers[Random.Next(volunteers.Count)];
            var additionalMovements = GenerateRandomMovementChain(randomVolunteer.Id, alanDisi, boo, cikis, operationalSectors, baseTime);
            movements.AddRange(additionalMovements);
            baseTime = baseTime.AddHours(Random.Next(1, 4));
        }

        return [.. movements.Take(10000)];
    }
    
    private static List<Movement> GenerateVolunteerMovementChain(Guid volunteerId, Sector entry, Sector hub, Sector exit, List<Sector> operationalSectors, DateTime startTime)
    {
        var movements = new List<Movement>();
        var currentTime = startTime.AddMinutes(Random.Next(0, 1440)); // Random time within the day
        
        // 1. İntikal: ALAN_DIŞI → BOO (mandatory first movement)
        movements.Add(Movement.Create(volunteerId, entry.Id, hub.Id, MovementType.Entry, false, null));
        currentTime = currentTime.AddMinutes(Random.Next(30, 180));
        
        // 2. Random transfers between operational sectors (through BOO)
        var transferCount = Random.Next(3, 12); // 3-12 transfers per volunteer
        var currentLocation = hub.Id;
        
        for (int i = 0; i < transferCount; i++)
        {
            var isGroupMovement = Random.NextDouble() < 0.15; // 15% chance of group movement
            Guid? groupId = isGroupMovement ? Guid.NewGuid() : null;
            
            if (currentLocation == hub.Id)
            {
                // From BOO to operational sector
                var targetSector = operationalSectors[Random.Next(operationalSectors.Count)];
                movements.Add(Movement.Create(volunteerId, currentLocation, targetSector.Id, MovementType.Transfer, 
                    isGroupMovement, groupId));
                currentLocation = targetSector.Id;
            }
            else
            {
                // From operational sector back to BOO
                movements.Add(Movement.Create(volunteerId, currentLocation, hub.Id, MovementType.Transfer, 
                    isGroupMovement, groupId));
                currentLocation = hub.Id;
            }
            
            currentTime = currentTime.AddMinutes(Random.Next(45, 240));
        }
        
        // 3. Ensure volunteer is at BOO for potential exit
        if (currentLocation != hub.Id)
        {
            movements.Add(Movement.Create(volunteerId, currentLocation, hub.Id, MovementType.Transfer, 
                false, null));
            currentTime = currentTime.AddMinutes(Random.Next(15, 60));
        }
        
        // 4. 70% chance of exit
        if (Random.NextDouble() < 0.7)
        {
            movements.Add(Movement.Create(volunteerId, hub.Id, exit.Id, MovementType.Exit, false, null));
        }
        
        return movements;
    }
    
    private static List<Movement> GenerateRandomMovementChain(Guid volunteerId, Sector entry, Sector hub, Sector exit, List<Sector> operationalSectors, DateTime startTime)
    {
        var movements = new List<Movement>();
        var currentTime = startTime.AddMinutes(Random.Next(0, 1440));
        
        // Quick movement chain for filling up to 10000
        var movementCount = Random.Next(1, 4);
        
        for (int i = 0; i < movementCount; i++)
        {
            var fromSector = i == 0 ? entry : (Random.NextDouble() < 0.6 ? hub : operationalSectors[Random.Next(operationalSectors.Count)]);
            var toSector = Random.NextDouble() < 0.4 ? hub : operationalSectors[Random.Next(operationalSectors.Count)];
            
            var movementType = i == 0 ? MovementType.Entry : 
                              (Random.NextDouble() < 0.1 ? MovementType.Exit : MovementType.Transfer);
            
            movements.Add(Movement.Create(volunteerId, fromSector.Id, toSector.Id, movementType, 
                Random.NextDouble() < 0.1, Random.NextDouble() < 0.1 ? Guid.NewGuid() : null));
            
            currentTime = currentTime.AddMinutes(Random.Next(30, 180));
        }
        
        return movements;
    }

    public static Movement GetEntryMovement(Guid volunteerId, Guid entryId, Guid hubId)
    {
        return Movement.Create(volunteerId, entryId, hubId, MovementType.Entry);
    }

    public static Movement GetTransferMovement(Guid volunteerId, Guid fromId, Guid toId)
    {
        return Movement.Create(volunteerId, fromId, toId, MovementType.Transfer);
    }

    public static Movement GetExitMovement(Guid volunteerId, Guid hubId, Guid exitId)
    {
        return Movement.Create(volunteerId, hubId, exitId, MovementType.Exit);
    }

    public static Movement GetGroupMovement(Guid volunteerId, Guid fromId, Guid toId, Guid groupId)
    {
        return Movement.Create(volunteerId, fromId, toId, MovementType.Transfer, true, groupId);
    }
}
