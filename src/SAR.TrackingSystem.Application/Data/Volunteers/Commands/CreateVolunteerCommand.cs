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
        // QR uniqueness validation
        var qrExists = await repository.ExistsByQRIdAsync(request.Request.QRId, null, cancellationToken);
        if (qrExists)
            throw new ValidationException("Bu QR ID zaten kullanımda.");

        var volunteer = Volunteer.Create(
            fullName: request.Request.FullName,
            teamId: request.Request.TeamId,
            qrId: request.Request.QRId,
            role: request.Request.Role);

        await repository.AddAsync(volunteer, cancellationToken);
        return volunteer.Id;
    }
}

public sealed class CreateVolunteerCommandValidator : AbstractValidator<CreateVolunteerCommand>
{
    public CreateVolunteerCommandValidator()
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
