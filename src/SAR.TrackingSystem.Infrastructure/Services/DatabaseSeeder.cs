using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Domain.SeedData;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SarDbContext context)
    {
        if (!await context.Teams.AnyAsync())
        {
            await context.Teams.AddRangeAsync(DefaultSeedData.Teams);
            await context.SaveChangesAsync();
        }
        
        if (!await context.Sectors.AnyAsync())
        {
            await context.Sectors.AddRangeAsync(DefaultSeedData.Sectors);
            await context.SaveChangesAsync();
        }
    }
}
