using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence;

public class SarDbContext(
    DbContextOptions<SarDbContext> options) : DbContext(options)
{
    public required DbSet<Volunteer> Volunteers { get; set; }
    public required DbSet<Team> Teams { get; set; }
    public required DbSet<Sector> Sectors { get; set; }
    public required DbSet<Movement> Movements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
