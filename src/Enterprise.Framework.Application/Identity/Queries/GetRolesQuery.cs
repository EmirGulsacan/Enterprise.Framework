namespace Enterprise.Framework.Application.Identity.Queries;

using AutoMapper;

using AutoMapper.QueryableExtensions;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities.Identity;

using Enterprise.Framework.Application.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Enterprise.Framework.Application.Common.Mappings;



public record GetRolesQuery : IRequest<List<RoleDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetRolesQueryHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken) {
    
return await _context.GetDbSet<AppRole>() {
            .AsNoTracking() {
            .OrderBy(x => x.Name) {
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider) {
            .ToListAsync(cancellationToken);

    }

}


}






}
}
}
}



