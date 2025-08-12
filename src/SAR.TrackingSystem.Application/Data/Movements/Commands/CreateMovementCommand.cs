using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;
using SAR.TrackingSystem.Domain.StateMachine;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record CreateMovementCommand(MovementRequest Request) : IRequest<Guid>;

public sealed class CreateMovementCommandHandler(
    IMovementRepository movementRepository,
    ISectorRepository sectorRepository,
    IVolunteerRepository volunteerRepository,
    IOptions<SectorConfiguration> config) : IRequestHandler<CreateMovementCommand, Guid>
{
    private readonly SectorConfiguration _config = config.Value;

    public async Task<Guid> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {
        // Get volunteer and validate existence
        var volunteer = await volunteerRepository.GetByIdAsync(request.Request.VolunteerId, cancellationToken) 
            ?? throw new ValidationException("Volunteer not found.");

        // Get target sector info
        string? targetSectorCode = null;
        if (request.Request.ToSectorId.HasValue)
        {
            var toSector = await sectorRepository.GetByIdAsync(request.Request.ToSectorId.Value, cancellationToken) ?? throw new ValidationException("Invalid target sector.");

            targetSectorCode = toSector.Code;
        }

        // STATE MACHINE VALIDATION - Use volunteer.CurrentState, ignore FromSectorId
        var targetState = targetSectorCode == null 
            ? VolunteerState.Exited
            : StateTransitions.GetStateFromSector(targetSectorCode, _config);
            
        if (!StateTransitions.IsValidTransition(volunteer.CurrentState, targetState))
        {
            var error = StateTransitions.GetTransitionError(volunteer.CurrentState, targetState);
            throw new ValidationException(error);
        }

        // Group movement validation
        if (request.Request.IsGroupMovement && !request.Request.GroupId.HasValue)
            throw new ValidationException("Grup hareketi için GroupId zorunludur.");

        // Determine correct FromSectorId based on current state
        Guid? correctFromSectorId = null;
        if (volunteer.CurrentState != VolunteerState.NotEntered)
        {
            var lastMovement = await movementRepository.GetLastMovementAsync(request.Request.VolunteerId, cancellationToken);
            correctFromSectorId = lastMovement?.ToSectorId;
        }

        // Create Movement with corrected FromSectorId
        var movement = Movement.Create(
            volunteerId: request.Request.VolunteerId,
            fromSectorId: correctFromSectorId, // Use calculated, not web input
            toSectorId: request.Request.ToSectorId,
            type: request.Request.Type,
            isGroupMovement: request.Request.IsGroupMovement,
            groupId: request.Request.GroupId,
            notes: request.Request.Notes);

        await movementRepository.AddAsync(movement, cancellationToken);
        
        // Update volunteer state
        volunteer.UpdateState(targetState);
        await volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        
        return movement.Id;
    }
}

// State Machine Validator
public sealed class CreateMovementCommandValidator : AbstractValidator<CreateMovementCommand>
{
    public CreateMovementCommandValidator()
    {
        RuleFor(x => x.Request.VolunteerId)
            .NotEmpty()
            .WithMessage("Volunteer must be selected.");
        
        RuleFor(x => x.Request.GroupId)
            .NotEmpty()
            .When(x => x.Request.IsGroupMovement)
            .WithMessage("Group ID is required for group movements.");
    }
}