using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Application.Data.Sectors.Queries;

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

        app.MapGet("/sectors", GetSectors)
            .WithName(nameof(GetSectors))
            .WithOpenApi(operation =>
            {
                operation.Summary = "List all sectors";
                operation.Description = "Retrieves all sectors for dropdown/selection.";
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

    private static async Task<Ok<List<SectorResponse>>> GetSectors(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetSectorsQuery(),
            context.RequestAborted);

        return TypedResults.Ok(result);
    }
}