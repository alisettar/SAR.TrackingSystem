using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Application.Data.Dashboard.Queries;

namespace SAR.TrackingSystem.Api.Modules.Dashboard;

public class DashboardModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/stats", GetDashboardStats)
            .WithName(nameof(GetDashboardStats))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get dashboard statistics";
                operation.Description = "Retrieves volunteer state counts for dashboard display.";
                return operation;
            });

        app.MapGet("/dashboard/sector-distribution", GetSectorDistribution)
            .WithName(nameof(GetSectorDistribution))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get sector distribution";
                operation.Description = "Retrieves volunteer distribution across sectors.";
                return operation;
            });

        app.MapGet("/dashboard/city-distribution", GetCityDistribution)
            .WithName(nameof(GetCityDistribution))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get city distribution";
                operation.Description = "Retrieves volunteer distribution by city.";
                return operation;
            });

        app.MapGet("/dashboard/team-distribution", GetTeamDistribution)
            .WithName(nameof(GetTeamDistribution))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get team distribution";
                operation.Description = "Retrieves volunteer counts by team (arrived only).";
                return operation;
            });
    }

    private static async Task<Ok<DashboardStatsResponse>> GetDashboardStats(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var query = new GetDashboardStatsQuery();
        var result = await sender.Send(query, context.RequestAborted);
        
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<SectorDistributionResponse>> GetSectorDistribution(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var query = new GetSectorDistributionQuery();
        var result = await sender.Send(query, context.RequestAborted);
        
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CityDistributionResponse>> GetCityDistribution(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var query = new GetCityDistributionQuery();
        var result = await sender.Send(query, context.RequestAborted);
        
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<TeamDistributionResponse>> GetTeamDistribution(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var query = new GetTeamDistributionQuery();
        var result = await sender.Send(query, context.RequestAborted);
        
        return TypedResults.Ok(result);
    }
}
