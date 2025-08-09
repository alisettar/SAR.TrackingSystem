using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.QRId)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Volunteers)
            .HasForeignKey(x => x.TeamId);
            
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.Volunteer)
            .HasForeignKey(x => x.VolunteerId);

        // QRId unique index if provided
        builder.HasIndex(x => x.QRId)
            .IsUnique()
            .HasFilter("[QRId] IS NOT NULL");
    }
}
