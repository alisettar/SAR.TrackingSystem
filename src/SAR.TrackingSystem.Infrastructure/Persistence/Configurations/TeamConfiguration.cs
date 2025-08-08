using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.HasMany(x => x.Volunteers)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.TeamId);

        builder.HasIndex(x => x.Code).IsUnique();
    }
}
