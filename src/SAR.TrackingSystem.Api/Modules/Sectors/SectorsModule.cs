using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Application.Data.Sectors.Commands;
using SAR.TrackingSystem.Application.Data.Sectors.Queries;
using SAR.TrackingSystem.Application.Data.Sectors.Statistics;

namespace SAR.TrackingSystem.Api.Modules.Sectors;

public class SectorsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sectors/{id}", GetSectorById)
            .WithName(nameof(GetSectorById))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get sector by ID";
                operation.Description = "Retrieves a specific sector by ID.";
                return operation;
            });

        app.MapGet("/sectors/{id}/statistics", GetSectorStatistics)
            .WithName(nameof(GetSectorStatistics))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get sector statistics";
                operation.Description = "Gets real-time statistics for a specific sector.";
                return operation;
            });

        app.MapGet("/sectors", GetSectors)
            .WithName(nameof(GetSectors))
            .WithOpenApi(operation =>
            {
                operation.Summary = "List all sectors";
                operation.Description = "Retrieves all sectors for dropdown/selection.";
                return operation;
            });

        app.MapPost("/sectors", CreateSector)
            .WithName(nameof(CreateSector))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Create new sector";
                operation.Description = "Creates a new sector with the provided data.";
                return operation;
            });

        app.MapDelete("/sectors/{id}", DeleteSector)
            .WithName(nameof(DeleteSector))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Delete sector";
                operation.Description = "Deletes a sector by ID. Critical sectors cannot be deleted.";
                return operation;
            });
    }

    private static async Task<Results<Ok<SectorResponse>, NotFound>> GetSectorById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new GetSectorByIdQuery(id);
        var sector = await sender.Send(request, context.RequestAborted);

        return sector is not null
            ? TypedResults.Ok(sector)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<SectorStatisticsResponse>, NotFound>> GetSectorStatistics(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new GetSectorStatisticsQuery(id);
        var statistics = await sender.Send(request, context.RequestAborted);

        return TypedResults.Ok(statistics);
    }

    private static async Task<Ok<List<SectorResponse>>> GetSectors(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetSectorsQuery(),
            context.RequestAborted);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created, ValidationProblem>> CreateSector(
        SectorRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var command = new CreateSectorCommand(request);
        var sectorId = await sender.Send(command, context.RequestAborted);

        return TypedResults.Created($"/sectors/{sectorId}");
    }

    private static async Task<Results<Ok, NotFound, ValidationProblem>> DeleteSector(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        try
        {
            var command = new DeleteSectorCommand(id);
            var result = await sender.Send(command, context.RequestAborted);

            return result ? TypedResults.Ok() : TypedResults.NotFound();
        }
        catch (ValidationException ex)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                ["error"] = [ex.Message]
            });
        }
    }
}