using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using SAR.TrackingSystem.Application.Behaviors;

namespace SAR.TrackingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddLogging(); // ILogger servislerini ekler

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}