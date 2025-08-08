using Microsoft.Extensions.DependencyInjection;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Infrastructure.Repositories;

namespace SAR.TrackingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repository registrations
        services.AddScoped<IVolunteerRepository, VolunteerRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();

        return services;
    }
}