using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TcKimlik)
            .IsRequired();
            
        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.BloodType)
            .HasMaxLength(20);
            
        builder.Property(x => x.Phone)
            .HasMaxLength(50);
            
        builder.Property(x => x.EmergencyContactName)
            .HasMaxLength(200);
            
        builder.Property(x => x.EmergencyContactPhone)
            .HasMaxLength(50);
            
        builder.Property(x => x.Buddy1)
            .HasMaxLength(200);
            
        builder.Property(x => x.Buddy2)
            .HasMaxLength(200);

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Volunteers)
            .HasForeignKey(x => x.TeamId);
            
        builder.HasMany(x => x.Movements)
            .WithOne(x => x.Volunteer)
            .HasForeignKey(x => x.VolunteerId);

        builder.HasIndex(x => x.TcKimlik).IsUnique();
    }
}
