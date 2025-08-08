using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence;

public class SarDbContext(
    DbContextOptions<SarDbContext> options) : DbContext(options)
{
    public DbSet<Volunteer> Volunteers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Movement> Movements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
