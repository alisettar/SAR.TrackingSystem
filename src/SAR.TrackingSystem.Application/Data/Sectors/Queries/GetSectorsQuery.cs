using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Sectors.Queries;

public sealed record GetSectorsQuery : IRequest<List<SectorResponse>>;

public sealed class GetSectorsQueryHandler(ISectorRepository repository) 
    : IRequestHandler<GetSectorsQuery, List<SectorResponse>>
{
    public async Task<List<SectorResponse>> Handle(GetSectorsQuery request, CancellationToken cancellationToken)
    {
        var sectors = await repository.GetAllAsync(cancellationToken);
        return SectorResponse.FromDomainList(sectors);
    }
}

public sealed record GetSectorByIdQuery(Guid Id) : IRequest<SectorResponse?>;

public sealed class GetSectorByIdQueryHandler(ISectorRepository repository)
    : IRequestHandler<GetSectorByIdQuery, SectorResponse?>
{
    public async Task<SectorResponse?> Handle(GetSectorByIdQuery request, CancellationToken cancellationToken)
    {
        var sector = await repository.GetByIdAsync(request.Id, cancellationToken);
        return sector == null ? null : SectorResponse.FromDomain(sector);
    }
}