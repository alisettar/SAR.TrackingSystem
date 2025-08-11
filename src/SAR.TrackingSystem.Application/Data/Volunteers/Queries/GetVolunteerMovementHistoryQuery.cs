using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Application.Data.Volunteers.Queries;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetVolunteerMovementHistoryQuery(Guid VolunteerId) : IRequest<List<MovementHistoryResponse>>;

public sealed class GetVolunteerMovementHistoryQueryHandler(IMovementRepository repository) 
    : IRequestHandler<GetVolunteerMovementHistoryQuery, List<MovementHistoryResponse>>
{
    public async Task<List<MovementHistoryResponse>> Handle(GetVolunteerMovementHistoryQuery request, CancellationToken cancellationToken)
    {
        var movements = await repository.GetByVolunteerIdAsync(request.VolunteerId, cancellationToken);
        return movements
            .Select(MovementHistoryResponse.FromDomain)
            .OrderByDescending(m => m.MovementTime)
            .ToList();
    }
}

public sealed record MovementHistoryResponse
{
    public Guid Id { get; init; }
    public DateTime MovementTime { get; init; }
    public string FromSector { get; init; } = string.Empty;
    public string ToSector { get; init; } = string.Empty;
    public string MovementType { get; init; } = string.Empty;
    public bool IsGroupMovement { get; init; }
    public string Notes { get; init; } = string.Empty;

    public static MovementHistoryResponse FromDomain(Domain.Entities.Movement movement)
    {
        return new MovementHistoryResponse
        {
            Id = movement.Id,
            MovementTime = movement.MovementTime,
            FromSector = movement.FromSector?.Code ?? "ALAN_DIŞI",
            ToSector = movement.ToSector?.Code ?? "Çıkış",
            MovementType = GetMovementTypeDescription(movement),
            IsGroupMovement = movement.IsGroupMovement,
            Notes = movement.Notes ?? ""
        };
    }

    private static string GetMovementTypeDescription(Domain.Entities.Movement movement)
    {
        if (movement.FromSector == null) return "Giriş";
        if (movement.ToSector == null) return "Çıkış";
        return "Transfer";
    }
}
