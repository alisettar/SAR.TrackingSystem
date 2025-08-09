using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Domain.SeedData;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SarDbContext context)
    {
        // Seed Teams with upsert logic
        foreach (var team in DefaultSeedData.Teams)
        {
            var existingTeam = await context.Teams.FirstOrDefaultAsync(t => t.Code == team.Code);
            if (existingTeam == null)
            {
                context.Teams.Add(team);
            }
        }
        await context.SaveChangesAsync();
        
        // Seed Sectors with upsert logic
        foreach (var sector in DefaultSeedData.Sectors)
        {
            var existingSector = await context.Sectors.FirstOrDefaultAsync(s => s.Code == sector.Code);
            if (existingSector == null)
            {
                context.Sectors.Add(sector);
            }
        }
        await context.SaveChangesAsync();
    }
}
