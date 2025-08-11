using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Volunteers.Commands;
using SAR.TrackingSystem.Application.Data.Volunteers.Queries;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Api.Modules.Volunteers;

public class VolunteersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/volunteers/{id}", GetVolunteerById)
            .WithName(nameof(GetVolunteerById))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get volunteer by ID";
                operation.Description = "Retrieves a specific volunteer by ID.";
                return operation;
            });

        app.MapGet("/volunteers", GetVolunteers)
            .WithName(nameof(GetVolunteers))
            .WithOpenApi(operation =>
            {
                operation.Summary = "List volunteers with pagination";
                operation.Description = "Retrieves paginated list of volunteers.";
                return operation;
            });

        app.MapPost("/volunteers", CreateVolunteer)
            .WithName(nameof(CreateVolunteer))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Create new volunteer";
                operation.Description = "Creates a new volunteer entry.";
                return operation;
            });

        app.MapPut("/volunteers/{id}", UpdateVolunteer)
            .WithName(nameof(UpdateVolunteer))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Update volunteer";
                operation.Description = "Updates existing volunteer.";
                return operation;
            });

        app.MapDelete("/volunteers/{id}", DeleteVolunteer)
            .WithName(nameof(DeleteVolunteer))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Delete volunteer";
                operation.Description = "Deletes volunteer by ID.";
                return operation;
            });

        app.MapGet("/volunteers/{id}/movements", GetVolunteerMovementHistory)
            .WithName(nameof(GetVolunteerMovementHistory))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get volunteer movement history";
                operation.Description = "Retrieves timeline of all movements for a volunteer.";
                return operation;
            });
    }

    private static async Task<Results<Ok<VolunteerResponse>, NotFound>> GetVolunteerById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new GetVolunteerByIdQuery(id);
        var volunteer = await sender.Send(request, context.RequestAborted);

        return volunteer is not null
            ? TypedResults.Ok(volunteer)
            : TypedResults.NotFound();
    }

    private static async Task<Ok<PaginationResponse<VolunteerResponse>>> GetVolunteers(
        [FromQuery] PaginationRequest? paginationRequest,
        [FromQuery] string? filter,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] PaginationRequest? bodyPaginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        // Priority: Body > Query > Default
        paginationRequest = bodyPaginationRequest ?? paginationRequest;

        // Parse filter parameter
        VolunteerState? stateFilter = filter switch
        {
            "inHub" => VolunteerState.InHub,
            "inSector" => VolunteerState.InSector,
            "exited" => VolunteerState.Exited,
            "notEntered" => VolunteerState.NotEntered,
            _ => null
        };

        var result = await sender.Send(
            new GetVolunteersQuery(paginationRequest ?? new(), stateFilter),
            context.RequestAborted);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created, ValidationProblem>> CreateVolunteer(
        VolunteerRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var command = new CreateVolunteerCommand(request);
        var volunteerId = await sender.Send(command, context.RequestAborted);

        return TypedResults.Created($"/volunteers/{volunteerId}");
    }

    private static async Task<Results<Ok, NotFound, ValidationProblem>> UpdateVolunteer(
        Guid id,
        VolunteerRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var requestWithId = request with { Id = id };
        var command = new UpdateVolunteerCommand(requestWithId);
        _ = await sender.Send(command, context.RequestAborted);

        return TypedResults.Ok();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteVolunteer(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var command = new DeleteVolunteerCommand(id);
        var result = await sender.Send(command, context.RequestAborted);

        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Ok<List<MovementHistoryResponse>>> GetVolunteerMovementHistory(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var query = new GetVolunteerMovementHistoryQuery(id);
        var movements = await sender.Send(query, context.RequestAborted);

        return TypedResults.Ok(movements);
    }
}