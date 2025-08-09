using FluentValidation;
using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Commands;

public sealed record UpdateVolunteerCommand(VolunteerRequest Request) : IRequest<Guid>;

public sealed class UpdateVolunteerCommandHandler(IVolunteerRepository repository) 
    : IRequestHandler<UpdateVolunteerCommand, Guid>
{
    public async Task<Guid> Handle(UpdateVolunteerCommand request, CancellationToken cancellationToken)
    {
        var volunteer = await repository.GetByIdAsync(request.Request.Id, cancellationToken) 
            ?? throw new ValidationException("Volunteer not found");

        // QR uniqueness validation
        var qrExists = await repository.ExistsByQRIdAsync(request.Request.QRId, request.Request.Id, cancellationToken);
        if (qrExists)
            throw new ValidationException("Bu QR ID zaten kullanımda.");

        var updatedVolunteer = Volunteer.Update(
            currentVolunteer: volunteer,
            fullName: request.Request.FullName,
            teamId: request.Request.TeamId,
            qrId: request.Request.QRId,
            role: request.Request.Role);

        await repository.UpdateAsync(updatedVolunteer, cancellationToken);
        return updatedVolunteer.Id;
    }
}

public sealed class UpdateVolunteerCommandValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerCommandValidator()
    {
        RuleFor(x => x.Request.FullName)
            .NotEmpty()
            .WithMessage("Full name cannot be empty.");
        
        RuleFor(x => x.Request.TeamId)
            .NotEmpty()
            .WithMessage("Team must be selected.");
            
        RuleFor(x => x.Request.QRId)
            .NotEmpty()
            .WithMessage("QR ID cannot be empty.");
            
        RuleFor(x => x.Request.Role)
            .NotEmpty()
            .WithMessage("Role cannot be empty.");
    }
}
