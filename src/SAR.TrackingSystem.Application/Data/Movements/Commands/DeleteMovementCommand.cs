using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using FluentValidation;
using Microsoft.Extensions.Options;
using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Domain.StateMachine;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record DeleteMovementCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteMovementCommandHandler(
    IMovementRepository movementRepository,
    IVolunteerRepository volunteerRepository,
    IOptions<SectorConfiguration> config) 
    : IRequestHandler<DeleteMovementCommand, bool>
{
    private readonly SectorConfiguration _config = config.Value;
    
    public async Task<bool> Handle(DeleteMovementCommand request, CancellationToken cancellationToken)
    {
        // Get movement to find volunteer before deletion
        var movement = await movementRepository.GetByIdAsync(request.Id, cancellationToken);
        if (movement == null) return false;
        
        var volunteerId = movement.VolunteerId;
        
        // Delete movement
        var deleted = await movementRepository.DeleteAsync(request.Id, cancellationToken);
        
        if (deleted)
        {
            // Recalculate volunteer state from remaining movements
            var volunteer = await volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer != null)
            {
                var lastMovement = await movementRepository.GetLastMovementAsync(volunteerId, cancellationToken);
                
                var newState = lastMovement == null 
                    ? VolunteerState.NotEntered
                    : StateTransitions.GetStateFromSector(lastMovement.ToSector?.Code, _config);
                    
                // Special case: Exit movement (null ToSector)
                if (lastMovement?.ToSectorId == null && lastMovement != null)
                    newState = VolunteerState.Exited;
                
                volunteer.UpdateState(newState);
                await volunteerRepository.UpdateAsync(volunteer, cancellationToken);
            }
        }
        
        return deleted;
    }
}

public sealed class DeleteMovementCommandValidator : AbstractValidator<DeleteMovementCommand>
{
    public DeleteMovementCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Movement ID cannot be empty.");
    }
}
