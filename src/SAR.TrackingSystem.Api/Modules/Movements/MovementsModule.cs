using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Movements.Commands;
using SAR.TrackingSystem.Application.Data.Movements.Queries;

namespace SAR.TrackingSystem.Api.Modules.Movements;

public class MovementsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/movements/{id}", GetMovementById)
            .WithName(nameof(GetMovementById))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get movement by ID";
                operation.Description = "Retrieves a specific movement by ID.";
                return operation;
            });

        app.MapGet("/movements", GetMovements)
            .WithName(nameof(GetMovements))
            .WithOpenApi(operation =>
            {
                operation.Summary = "List movements with pagination";
                operation.Description = "Retrieves paginated list of movements.";
                return operation;
            });

        app.MapPost("/movements", CreateMovement)
            .WithName(nameof(CreateMovement))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Create new movement";
                operation.Description = "Creates a new movement entry.";
                return operation;
            });
    }

    private static async Task<Results<Ok<MovementResponse>, NotFound>> GetMovementById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new GetMovementByIdQuery(id);
        var movement = await sender.Send(request, context.RequestAborted);

        return movement is not null
            ? TypedResults.Ok(movement)
            : TypedResults.NotFound();
    }

    private static async Task<Ok<PaginationResponse<MovementResponse>>> GetMovements(
        [FromQuery] PaginationRequest? paginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetMovementsQuery(paginationRequest),
            context.RequestAborted);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created, ValidationProblem>> CreateMovement(
        MovementRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var command = new CreateMovementCommand(request);
        var movementId = await sender.Send(command, context.RequestAborted);

        return TypedResults.Created($"/movements/{movementId}");
    }
}