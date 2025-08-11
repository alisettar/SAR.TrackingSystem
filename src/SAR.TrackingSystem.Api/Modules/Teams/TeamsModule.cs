using Carter;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Teams.Commands;
using SAR.TrackingSystem.Application.Data.Teams.Queries;

namespace SAR.TrackingSystem.Api.Modules.Teams;

public class TeamsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/teams/{id}", GetTeamById)
            .WithName(nameof(GetTeamById))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get team by ID";
                operation.Description = "Retrieves a specific team by ID.";
                return operation;
            });

        app.MapGet("/teams", GetTeams)
            .WithName(nameof(GetTeams))
            .WithOpenApi(operation =>
            {
                operation.Summary = "List all teams";
                operation.Description = "Retrieves all teams for dropdown/selection.";
                return operation;
            });

        app.MapPost("/teams", CreateTeam)
            .WithName(nameof(CreateTeam))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Create new team";
                operation.Description = "Creates a new team with the provided data.";
                return operation;
            });

        app.MapGet("/teams/{id}/members", GetTeamMembers)
            .WithName(nameof(GetTeamMembers))
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get team members (paginated)";
                operation.Description = "Retrieves team members with pagination.";
                return operation;
            });
    }

    private static async Task<Results<Ok<TeamResponse>, NotFound>> GetTeamById(
        Guid id,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var request = new GetTeamByIdQuery(id);
        var team = await sender.Send(request, context.RequestAborted);

        return team is not null
            ? TypedResults.Ok(team)
            : TypedResults.NotFound();
    }

    private static async Task<Ok<List<TeamResponse>>> GetTeams(
        [FromServices] ISender sender,
        HttpContext context)
    {
        var result = await sender.Send(
            new GetTeamsQuery(),
            context.RequestAborted);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created, ValidationProblem>> CreateTeam(
        TeamRequest request,
        [FromServices] ISender sender,
        HttpContext context)
    {
        var command = new CreateTeamCommand(request);
        var teamId = await sender.Send(command, context.RequestAborted);

        return TypedResults.Created($"/teams/{teamId}");
    }

    private static async Task<Ok<PaginationResponse<TeamMemberResponse>>> GetTeamMembers(
        Guid id,
        [FromQuery] PaginationRequest? paginationRequest,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] PaginationRequest? bodyPaginationRequest,
        [FromServices] ISender sender,
        HttpContext context)
    {
        // Priority: Body > Query > Default
        paginationRequest = bodyPaginationRequest ?? paginationRequest;

        var request = new GetTeamMembersQuery(id, paginationRequest ?? new());
        var result = await sender.Send(request, context.RequestAborted);

        return TypedResults.Ok(result);
    }
}