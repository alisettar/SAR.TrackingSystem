using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.HasMany(x => x.MovementsFrom)
            .WithOne(x => x.FromSector)
            .HasForeignKey(x => x.FromSectorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(x => x.MovementsTo)
            .WithOne(x => x.ToSector)
            .HasForeignKey(x => x.ToSectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
