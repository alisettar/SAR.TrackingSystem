using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using FluentValidation;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Commands;

public sealed record CreateVolunteerCommand(VolunteerRequest Request) : IRequest<Guid>;

public sealed class CreateVolunteerCommandHandler(IVolunteerRepository repository) 
    : IRequestHandler<CreateVolunteerCommand, Guid>
{
    public async Task<Guid> Handle(CreateVolunteerCommand request, CancellationToken cancellationToken)
    {
        var volunteer = Volunteer.Create(
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

        await repository.AddAsync(volunteer, cancellationToken);
        return volunteer.Id;
    }
}

public sealed class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
    {
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
