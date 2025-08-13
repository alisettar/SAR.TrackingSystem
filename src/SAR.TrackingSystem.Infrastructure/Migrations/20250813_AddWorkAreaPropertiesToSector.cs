using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Migrations;

public static class Migration_20250813_AddWorkAreaPropertiesToSector
{
    public static void Apply(SarDbContext context)
    {
        // Add new columns to Sectors table
        context.Database.ExecuteSqlRaw(@"
            ALTER TABLE Sectors ADD COLUMN WorkAreaName TEXT NOT NULL DEFAULT '';
            ALTER TABLE Sectors ADD COLUMN WorkAreaAddress TEXT NOT NULL DEFAULT '';
            ALTER TABLE Sectors ADD COLUMN Coordinates TEXT NOT NULL DEFAULT '';
            ALTER TABLE Sectors ADD COLUMN WorkAreaNumber INTEGER NOT NULL DEFAULT 0;
            ALTER TABLE Sectors ADD COLUMN ExpectedVictimCount INTEGER NOT NULL DEFAULT 0;
        ");

        // Save changes
        context.SaveChanges();
    }

    public static void Rollback(SarDbContext context)
    {
        // Drop the added columns if needed for rollback
        context.Database.ExecuteSqlRaw(@"
            ALTER TABLE Sectors DROP COLUMN WorkAreaName;
            ALTER TABLE Sectors DROP COLUMN WorkAreaAddress;
            ALTER TABLE Sectors DROP COLUMN Coordinates;
            ALTER TABLE Sectors DROP COLUMN WorkAreaNumber;
            ALTER TABLE Sectors DROP COLUMN ExpectedVictimCount;
        ");

        context.SaveChanges();
    }
}
