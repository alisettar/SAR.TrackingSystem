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

        var updatedVolunteer = Volunteer.Update(
            currentVolunteer: volunteer,
            tcKimlik: request.Request.TcKimlik,
            fullName: request.Request.FullName,
            teamId: request.Request.TeamId,
            bloodType: request.Request.BloodType,
            phone: request.Request.Phone,
            emergencyContactName: request.Request.EmergencyContactName,
            emergencyContactPhone: request.Request.EmergencyContactPhone,
            buddy1: request.Request.Buddy1,
            buddy2: request.Request.Buddy2,
            isActive: request.Request.IsActive);

        await repository.UpdateAsync(updatedVolunteer, cancellationToken);
        return updatedVolunteer.Id;
    }
}

public sealed class UpdateVolunteerCommandValidator : AbstractValidator<UpdateVolunteerCommand>
{
    public UpdateVolunteerCommandValidator()
    {
        RuleFor(x => x.Request.Id)
            .NotEmpty()
            .WithMessage("ID cannot be empty.");

        RuleFor(x => x.Request.TcKimlik)
            .NotEmpty()
            .WithMessage("TC Kimlik cannot be empty.");
        
        RuleFor(x => x.Request.FullName)
            .NotEmpty()
            .WithMessage("Full name cannot be empty.");
        
        RuleFor(x => x.Request.TeamId)
            .NotEmpty()
            .WithMessage("Team must be selected.");
    }
}