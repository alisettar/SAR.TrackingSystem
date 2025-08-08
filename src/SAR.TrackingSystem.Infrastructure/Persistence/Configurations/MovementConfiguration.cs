using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Type)
            .IsRequired();
            
        builder.Property(x => x.Notes)
            .HasMaxLength(500);
            
        builder.HasOne(x => x.Volunteer)
            .WithMany(x => x.Movements)
            .HasForeignKey(x => x.VolunteerId);
            
        builder.HasOne(x => x.FromSector)
            .WithMany(x => x.MovementsFrom)
            .HasForeignKey(x => x.FromSectorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(x => x.ToSector)
            .WithMany(x => x.MovementsTo)
            .HasForeignKey(x => x.ToSectorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.MovementTime);
        builder.HasIndex(x => x.GroupId).HasFilter("[GroupId] IS NOT NULL");
    }
}
