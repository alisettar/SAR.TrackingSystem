using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class MovementMockFactory
{
    public static List<Movement> GetSampleMovements(Guid volunteerId, List<Sector> sectors)
    {
        var alanDisi = sectors.First(s => s.Code == "ALAN_DIŞI");
        var boo = sectors.First(s => s.Code == "BOO");
        var e1 = sectors.First(s => s.Code == "E-1");

        return
        [
            // İntikal: ALAN_DIŞI → BOO
            Movement.Create(volunteerId, alanDisi.Id, boo.Id, MovementType.Entry),
            
            // Transfer: BOO → E-1
            Movement.Create(volunteerId, boo.Id, e1.Id, MovementType.Transfer),
            
            // Transfer: E-1 → BOO
            Movement.Create(volunteerId, e1.Id, boo.Id, MovementType.Transfer)
        ];
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
